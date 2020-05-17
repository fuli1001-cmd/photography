using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EF.EntityConfigurations
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
