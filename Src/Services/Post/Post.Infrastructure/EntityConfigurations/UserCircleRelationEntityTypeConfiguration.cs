using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class UserCircleRelationEntityTypeConfiguration : IEntityTypeConfiguration<UserCircleRelation>
    {
        public void Configure(EntityTypeBuilder<UserCircleRelation> builder)
        {
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
