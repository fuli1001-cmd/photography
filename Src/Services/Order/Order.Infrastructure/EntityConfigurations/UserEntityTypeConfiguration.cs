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
            var ordersNavigation = builder.Metadata.FindNavigation(nameof(User.User1Orders));
            ordersNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
