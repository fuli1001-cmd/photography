using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EF.EntityConfigurations
{
    class PostEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.PostAggregate.Post>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.PostAggregate.Post> builder)
        {
            builder.Ignore(b => b.DomainEvents);

            //navigation properties
            var attachmentsNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.PostAggregate.Post.PostAttachments));
            attachmentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //Location value object persisted as owned entity
            builder.OwnsOne(o => o.Location);
        }
    }
}
