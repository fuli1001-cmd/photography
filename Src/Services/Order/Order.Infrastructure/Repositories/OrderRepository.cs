using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Order.Infrastructure.Repositories
{
    public class OrderRepository : EfRepository<Domain.AggregatesModel.OrderAggregate.Order, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext context) : base(context)
        {

        }

        public async Task<Domain.AggregatesModel.OrderAggregate.Order> GetbyDealIdAsync(Guid dealId)
        {
            var orders = await _context.Orders.Where(o => o.DealId == dealId).ToListAsync();
            if (orders.Count > 0)
                return orders[0];
            else
                return null;
        }
    }
}
