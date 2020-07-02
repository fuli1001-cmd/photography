using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class UserShareEntityTypeConfiguration : IEntityTypeConfiguration<UserShare>
    {
        public void Configure(EntityTypeBuilder<UserShare> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasIndex(us => new { us.UserId, us.PostId });
            builder.HasIndex(us => new { us.UserId, us.TagId });
        }
    }
}
