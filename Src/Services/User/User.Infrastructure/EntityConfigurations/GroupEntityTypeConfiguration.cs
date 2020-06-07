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
        }
    }
}
