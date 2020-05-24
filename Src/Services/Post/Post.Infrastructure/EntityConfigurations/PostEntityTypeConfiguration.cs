using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class PostEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.PostAggregate.Post>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.PostAggregate.Post> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(p => p.Score).HasDefaultValue(0);
            builder.Property(p => p.LikeCount).HasDefaultValue(0);
            builder.Property(p => p.ShareCount).HasDefaultValue(0);
            builder.Property(p => p.CommentCount).HasDefaultValue(0);
            builder.Property(p => p.Commentable).HasDefaultValue(true);
            builder.Property(p => p.ShareType).HasDefaultValue(ShareType.Allowed);
            builder.Property(p => p.ForwardType).HasDefaultValue(ForwardType.Allowed);
            builder.Property(p => p.Visibility).HasDefaultValue(Visibility.Public);
            builder.Property(p => p.Commentable).HasDefaultValue(true);
            builder.Property(p => p.ShowOriginalText).HasDefaultValue(true);

            //attachments navigation properties
            var attachmentsNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.PostAggregate.Post.PostAttachments));
            attachmentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //comments navigation properties
            var commentsNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.PostAggregate.Post.Comments));
            commentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            // self navigation properties
            builder.HasMany(p => p.ForwardingPosts).WithOne(p => p.ForwardedPost).HasForeignKey(p => p.ForwardedPostId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            // self navigation properties
            builder.HasMany(p => p.AppointmentedFromPosts).WithOne(p => p.AppointmentedToPost).HasForeignKey(p => p.AppointmentedToPostId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            // UserPostRelations navigation properties
            builder.HasMany(p => p.UserPostRelations).WithOne(pu => pu.Post).HasForeignKey(pu => pu.PostId).OnDelete(DeleteBehavior.Restrict);

            // AppointmentedUser navigation properties
            builder.HasOne(p => p.AppointmentedUser).WithMany(u => u.Appointments).HasForeignKey(p => p.AppointmentedUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
