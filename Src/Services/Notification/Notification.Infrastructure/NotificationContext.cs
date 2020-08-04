using Arise.DDD.Domain.SeedWork;
using Arise.DDD.Infrastructure.Data;
using Arise.DDD.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Photography.Services.Notification.API.Query.ViewModels;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using Photography.Services.Notification.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Notification.Infrastructure
{
    public class NotificationContext : BaseContext
    {
        public DbSet<Event> Events { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<UserRelation> UserRelations { get; set; }

        public DbSet<UnReadEventCountViewModel> UnReadEventCounts { get; set; }

        public NotificationContext(DbContextOptions<NotificationContext> options, IMediator mediator) : base(options, mediator) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PostEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRelationEntityTypeConfiguration());

            modelBuilder.Entity<UnReadEventCountViewModel>().HasNoKey();
        }
    }
}
