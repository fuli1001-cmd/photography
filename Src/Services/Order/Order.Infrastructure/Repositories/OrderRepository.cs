using Arise.DDD.Infrastructure;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.Infrastructure.Repositories
{
    public class OrderRepository : EfRepository<Domain.AggregatesModel.OrderAggregate.Order, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext context) : base(context)
        {

        }
    }
}
