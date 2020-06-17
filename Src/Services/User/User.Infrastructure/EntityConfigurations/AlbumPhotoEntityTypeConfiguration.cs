using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.AlbumPhotoAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EntityConfigurations
{
    class AlbumPhotoEntityTypeConfiguration : IEntityTypeConfiguration<AlbumPhoto>
    {
        public void Configure(EntityTypeBuilder<AlbumPhoto> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.DisplayName).IsRequired();
        }
    }
}
