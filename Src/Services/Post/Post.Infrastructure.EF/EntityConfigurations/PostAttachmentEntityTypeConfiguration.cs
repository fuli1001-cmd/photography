using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    class PostAttachmentEntityTypeConfiguration : IEntityTypeConfiguration<PostAttachment>
    {
        public void Configure(EntityTypeBuilder<PostAttachment> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(p => p.Url).IsRequired();
        }
    }
}
