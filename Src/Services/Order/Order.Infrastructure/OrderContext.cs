using Arise.DDD.Infrastructure.Data;
using Arise.DDD.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Order.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.Order.Infrastructure
{
    public class OrderContext : BaseContext
    {
        public DbSet<Domain.AggregatesModel.OrderAggregate.Order> Orders { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Attachment> Attachments { get; set; }


        public OrderContext(DbContextOptions<OrderContext> options, IMediator mediator) : base(options, mediator) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}
