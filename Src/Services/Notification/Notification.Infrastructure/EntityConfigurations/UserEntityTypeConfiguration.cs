using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            var raisedEventsNavigation = builder.Metadata.FindNavigation(nameof(User.RaisedEvents));
            raisedEventsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var receivedEventsNavigation = builder.Metadata.FindNavigation(nameof(User.ReceivedEvents));
            receivedEventsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(u => u.FollowedUsers).WithOne(ur => ur.FollowedUser).HasForeignKey(ur => ur.FollowedUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Followers).WithOne(ur => ur.Follower).HasForeignKey(ur => ur.FollowerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
