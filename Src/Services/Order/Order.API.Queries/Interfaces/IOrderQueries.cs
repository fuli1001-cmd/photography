using Photography.Services.Order.API.Query.ViewModels;
using Photography.Services.Order.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Order.API.Query.Interfaces
{
    public interface IOrderQueries
    {
        Task<List<OrderViewModel>> GetOrdersAsync(IEnumerable<OrderStatus> orderStatus);

        Task<OrderViewModel> GetOrderAsync(Guid orderId);

        Task<OrderViewModel> GetOrderByDealIdAsync(Guid dealId);
    }
}
