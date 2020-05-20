using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(c => c.Text).IsRequired();
            builder.HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(c => c.SubComments).WithOne(c => c.ParentComment).HasForeignKey(c => c.ParentCommentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
