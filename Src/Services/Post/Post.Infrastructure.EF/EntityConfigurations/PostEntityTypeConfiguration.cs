using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    class PostEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.PostAggregate.Post>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.PostAggregate.Post> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(p => p.Points).HasDefaultValue(0);
            builder.Property(p => p.LikeCount).HasDefaultValue(0);
            builder.Property(p => p.ShareCount).HasDefaultValue(0);
            builder.Property(p => p.CommentCount).HasDefaultValue(0);
            builder.Property(p => p.Timestamp).HasDefaultValue(DateTime.UtcNow);
            builder.Property(p => p.Commentable).HasDefaultValue(true);
            builder.Property(p => p.ShareType).HasDefaultValue(ShareType.Allowed);
            builder.Property(p => p.ForwardType).HasDefaultValue(ForwardType.Allowed);
            builder.Property(p => p.Visibility).HasDefaultValue(Visibility.Public);

            //navigation properties
            var attachmentsNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.PostAggregate.Post.PostAttachments));
            attachmentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //Location value object persisted as owned entity
            builder.OwnsOne(o => o.Location);
        }
    }
}
