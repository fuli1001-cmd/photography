using Arise.DDD.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using Photography.Services.User.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.Infrastructure
{
    public class UserContext : BaseContext
    {
        public DbSet<Domain.AggregatesModel.UserAggregate.User> Users { get; set; }

        public DbSet<UserRelation> UserRelations { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupUser> GroupUsers { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<AlbumPhoto> AlbumPhotos { get; set; }

        public UserContext(DbContextOptions<UserContext> options, IMediator mediator) : base(options, mediator) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRelationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AlbumEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AlbumPhotoEntityTypeConfiguration());
        }
    }
}
