using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    public class PostForUserRelationEntityTypeConfiguration : IEntityTypeConfiguration<PostForUser>
    {
        public void Configure(EntityTypeBuilder<PostForUser> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasKey(pu => new { pu.UserId, pu.PostId });
        }
    }
}
