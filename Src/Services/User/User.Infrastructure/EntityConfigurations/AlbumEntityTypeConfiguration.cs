using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.AlbumAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EntityConfigurations
{
    class AlbumEntityTypeConfiguration : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(a => a.Name).IsRequired();

            var albumPhotosNavigation = builder.Metadata.FindNavigation(nameof(Album.AlbumPhotos));
            albumPhotosNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
