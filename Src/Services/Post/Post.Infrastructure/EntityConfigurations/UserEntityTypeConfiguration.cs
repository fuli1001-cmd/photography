using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(u => u.Score).HasDefaultValue(0);

            //posts navigation properties
            var postsNavigation = builder.Metadata.FindNavigation(nameof(User.Posts));
            postsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //comments navigation properties
            var commentsNavigation = builder.Metadata.FindNavigation(nameof(User.Comments));
            commentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //appointments navigation properties
            var appointmentsNavigation = builder.Metadata.FindNavigation(nameof(User.Appointments));
            appointmentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(u => u.UserPostRelations).WithOne(pu => pu.User).HasForeignKey(pu => pu.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserCommentRelations).WithOne(pu => pu.User).HasForeignKey(pu => pu.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserCircleRelations).WithOne(pu => pu.User).HasForeignKey(pu => pu.UserId).OnDelete(DeleteBehavior.Restrict);

            var circlesNavigation = builder.Metadata.FindNavigation(nameof(User.Circles));
            circlesNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            ////followers navigation properties
            //var followersNavigation = builder.Metadata.FindNavigation(nameof(User.Followers));
            //followersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            ////followerUsers navigation properties
            //var followedUsersNavigation = builder.Metadata.FindNavigation(nameof(User.FollowedUsers));
            //followedUsersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(u => u.FollowedUsers).WithOne(ur => ur.FollowedUser).HasForeignKey(ur => ur.FollowedUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Followers).WithOne(ur => ur.Follower).HasForeignKey(ur => ur.FollowerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
