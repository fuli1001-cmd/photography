using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    public class UserShareEntityTypeConfiguration : IEntityTypeConfiguration<UserShare>
    {
        public void Configure(EntityTypeBuilder<UserShare> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasIndex(s => s.SharerId);
            builder.HasIndex(s => new { s.SharerId, s.PostId });
            builder.HasIndex(s => new { s.SharerId, s.PrivateTag });
            builder.HasOne(s => s.Sharer).WithMany(u => u.UserShares).HasForeignKey(s => s.SharerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
