using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    public class UserPostRelationEntityTypeConfiguration : IEntityTypeConfiguration<UserPostRelation>
    {
        public void Configure(EntityTypeBuilder<UserPostRelation> builder)
        {
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
