using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.Infrastructure.EntityConfigurations
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.OrderAggregate.Order>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.OrderAggregate.Order> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.Property(o => o.LocationName).IsRequired();
            builder.Property(o => o.Address).IsRequired();
            builder.HasOne(o => o.User1).WithMany(u => u.Orders).HasForeignKey(o => o.User1Id).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(o => o.User2).WithMany(u => u.Orders).HasForeignKey(o => o.User2Id).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(o => o.Payer).WithMany(u => u.Orders).HasForeignKey(o => o.PayerId).OnDelete(DeleteBehavior.Restrict);

            var attachmentsNavigation = builder.Metadata.FindNavigation(nameof(Domain.AggregatesModel.OrderAggregate.Order.Attachments));
            attachmentsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
