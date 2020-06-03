using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.EntityConfigurations
{
    class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            builder.HasOne(e => e.FromUser).WithMany(u => u.RaisedEvents).HasForeignKey(e => e.FromUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.ToUser).WithMany(u => u.ReceivedEvents).HasForeignKey(e => e.ToUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
