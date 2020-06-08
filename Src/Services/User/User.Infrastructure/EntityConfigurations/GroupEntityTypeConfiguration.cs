using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EntityConfigurations
{
    class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.GroupAggregate.Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasOne(g => g.Owner).WithMany(u => u.Groups).HasForeignKey(g => g.OwnerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(g => g.GroupUsers).WithOne(gu => gu.Group).HasForeignKey(gu => gu.GroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.Property(u => u.ChatServerGroupId).ValueGeneratedOnAdd();
            builder.Property(u => u.ChatServerGroupId).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            var groupUsersNavigation = builder.Metadata.FindNavigation(nameof(Group.GroupUsers));
            groupUsersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
