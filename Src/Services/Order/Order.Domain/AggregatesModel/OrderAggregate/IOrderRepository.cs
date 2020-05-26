using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Order.Domain.AggregatesModel.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetbyDealIdAsync(Guid dealId);
        Task<Order> GetOrderWithAttachmentsAsync(Guid orderId);
    }
}
