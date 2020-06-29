using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.EntityConfigurations
{
    class CircleEntityTypeConfiguration : IEntityTypeConfiguration<Circle>
    {
        public void Configure(EntityTypeBuilder<Circle> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasIndex(c => c.UserCount);
            builder.HasIndex(c => c.Name);
            builder.Property(c => c.Name).IsRequired();
            
            builder.HasMany(c => c.UserCircleRelations).WithOne(uc => uc.Circle).HasForeignKey(uc => uc.CircleId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Owner).WithMany(u => u.Circles).HasForeignKey(c => c.OwnerId).OnDelete(DeleteBehavior.Restrict);

            //posts navigation properties
            var postsNavigation = builder.Metadata.FindNavigation(nameof(Circle.Posts));
            postsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
