using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.UserAggregate.User>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.UserAggregate.User> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(u => u.Code).IsRequired();
            builder.Property(u => u.RealNameRegistrationStatus).HasDefaultValue(IdAuthStatus.NoIdCard);
            builder.Property(u => u.Score).HasDefaultValue(0);
            builder.Property(u => u.LikedCount).HasDefaultValue(0);
            builder.Property(u => u.FollowerCount).HasDefaultValue(0);
            builder.Property(u => u.FollowingCount).HasDefaultValue(0);
            builder.Property(u => u.PostCount).HasDefaultValue(0);
            builder.Property(u => u.AppointmentCount).HasDefaultValue(0);
            builder.Property(u => u.LikedPostCount).HasDefaultValue(0);
            builder.Property(u => u.OngoingOrderCount).HasDefaultValue(0);
            builder.Property(u => u.ChatServerUserId).ValueGeneratedOnAdd();
            builder.Property(u => u.ChatServerUserId).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            builder.HasMany(u => u.FollowedUsers).WithOne(ur => ur.FollowedUser).HasForeignKey(ur => ur.FollowedUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Followers).WithOne(ur => ur.Follower).HasForeignKey(ur => ur.FollowerId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.GroupUsers).WithOne(gu => gu.User).HasForeignKey(gu => gu.UserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            var groupUsersNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.UserAggregate.User.GroupUsers));
            groupUsersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var groupsNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.UserAggregate.User.Groups));
            groupsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
