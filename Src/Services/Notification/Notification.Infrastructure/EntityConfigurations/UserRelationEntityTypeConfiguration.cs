using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.EntityConfigurations
{
    public class UserRelationEntityTypeConfiguration : IEntityTypeConfiguration<UserRelation>
    {
        public void Configure(EntityTypeBuilder<UserRelation> builder)
        {
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
