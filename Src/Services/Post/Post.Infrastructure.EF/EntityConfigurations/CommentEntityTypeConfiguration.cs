using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Ignore(b => b.DomainEvents);
            builder.Property<string>("Text").IsRequired();
        }
    }
}
