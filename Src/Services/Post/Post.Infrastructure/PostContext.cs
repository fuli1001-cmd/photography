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
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using Arise.DDD.Infrastructure.Data;
using Photography.Services.Post.API.Query.ViewModels;

namespace Photography.Services.Post.Infrastructure
{
    public class PostContext : BaseContext
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

        public DbSet<UserCircleRelation> UserCircleRelations { get; set; }

        public DbSet<UserShare> UserShares { get; set; }

        public DbSet<SentAndReceivedAppointmentDealCountViewModel> SentAndReceivedAppointmentDealCounts { get; set; }


        public PostContext(DbContextOptions<PostContext> options, IMediator mediator) : base(options, mediator) { }

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
            modelBuilder.ApplyConfiguration(new UserCircleRelationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserShareEntityTypeConfiguration());

            modelBuilder.Entity<SentAndReceivedAppointmentDealCountViewModel>().HasNoKey();

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
    }
}
