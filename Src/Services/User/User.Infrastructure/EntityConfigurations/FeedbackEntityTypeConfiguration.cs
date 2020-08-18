using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Services.User.Domain.AggregatesModel.FeedbackAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Infrastructure.EntityConfigurations
{
    class FeedbackEntityTypeConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
