using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Microsoft.EntityFrameworkCore.Storage;
using Photography.Services.Post.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Arise.DDD.Domain.SeedWork;
using Arise.DDD.Infrastructure.Extensions;
using MediatR;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;

namespace Photography.Services.Post.Infrastructure
{
    public class PostContext : DbContext, IUnitOfWork
    {
        public DbSet<Domain.AggregatesModel.PostAggregate.Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRelation> UserRelations { get; set; }
        public DbSet<UserPostRelation> UserPostRelations { get; set; }
        public DbSet<UserCommentRelation> UserCommentRelations { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Circle> Circles { get; set; }


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
            modelBuilder.ApplyConfiguration(new UserRelationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserPostRelationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TagEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CircleEntityTypeConfiguration());

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
