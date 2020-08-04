using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class TagEntityTypeConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(t => t.Name).IsRequired();
            builder.HasIndex(t => t.Name);
            builder.HasIndex(t => t.Count);
            builder.HasIndex(t => t.CreatedTime);
        }
    }
}
