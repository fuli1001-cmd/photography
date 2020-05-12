using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(u => u.Code).IsRequired();
            builder.Property(u => u.RealNameRegistered).HasDefaultValue(false);
            builder.Property(u => u.UserType).HasDefaultValue(UserType.Unknown);
            builder.Property(u => u.Points).HasDefaultValue(0);

            //navigation properties
            var postsNavigation = builder.Metadata.FindNavigation(nameof(User.Posts));
            postsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
