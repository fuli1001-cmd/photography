using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            //posts navigation properties
            var user1OrdersNavigation = builder.Metadata.FindNavigation(nameof(User.User1Orders));
            user1OrdersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var user2OrdersNavigation = builder.Metadata.FindNavigation(nameof(User.User2Orders));
            user2OrdersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var payerOrdersNavigation = builder.Metadata.FindNavigation(nameof(User.PayerOrders));
            payerOrdersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
