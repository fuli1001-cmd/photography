﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EF.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.UserAggregate.User>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.UserAggregate.User> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(u => u.Code).IsRequired();
            builder.Property(u => u.RealNameRegistered).HasDefaultValue(false);
            builder.Property(u => u.Score).HasDefaultValue(0);
            builder.Property(u => u.LikedCount).HasDefaultValue(0);
            builder.Property(u => u.FollowerCount).HasDefaultValue(0);
            builder.Property(u => u.FollowingCount).HasDefaultValue(0);
        }
    }
}
