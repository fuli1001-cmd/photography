using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Notification.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.EntityConfigurations
{
    public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            var eventsNavigation = builder.Metadata.FindNavigation(nameof(Post.Events));
            eventsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
