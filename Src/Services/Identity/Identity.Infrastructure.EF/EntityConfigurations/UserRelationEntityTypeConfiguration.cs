using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Identity.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Identity.Infrastructure.EF.EntityConfigurations
{
    public class UserRelationEntityTypeConfiguration : IEntityTypeConfiguration<UserRelation>
    {
        public void Configure(EntityTypeBuilder<UserRelation> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasKey(ur => new { ur.FollowedUserId, ur.FollowerId });
        }
    }
}
