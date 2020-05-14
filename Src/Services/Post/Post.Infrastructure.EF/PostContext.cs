using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Photography.Services.Post.Infrastructure.EF.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Photography.Services.Post.Infrastructure.EF.Extensions;
using System.Data;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;

namespace Photography.Services.Post.Infrastructure.EF
{
    public class PostContext : DbContext, IUnitOfWork
    {
        public DbSet<Domain.AggregatesModel.PostAggregate.Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRelation> UserRelations { get; set; }
        public DbSet<PostForUser> PostsForUsers { get; set; }


        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        private PostContext(DbContextOptions<PostContext> options) : base(options) { }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public PostContext(DbContextOptions<PostContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PostEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CommentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PostAttachmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            //foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    string tableName = entityType.GetTableName();
            //    entityType.SetTableName(tableName.ToLower());
            //    entityType.GetProperties().ToList().ForEach(p => p.SetColumnName(p.GetColumnName().ToLower()));
            //}

            //modelBuilder.Model.GetEntityTypes().ToList()
            //    .ForEach(e =>
            //    {
            //        e.SetTableName(e.GetTableName().ToLower());
            //        e.GetProperties().ToList().ForEach(p => p.SetColumnName(p.GetColumnName().ToLower()));
            //    });
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
