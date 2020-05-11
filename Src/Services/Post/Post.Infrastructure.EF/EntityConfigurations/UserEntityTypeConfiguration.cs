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
            builder.Ignore(b => b.DomainEvents);
            builder.Property<string>("Code").IsRequired();

            //navigation properties
            var postsNavigation = builder.Metadata.FindNavigation(nameof(User.Posts));
            postsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
