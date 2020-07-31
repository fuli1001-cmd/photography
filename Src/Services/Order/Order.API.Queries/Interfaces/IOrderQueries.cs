using Arise.DDD.API.Paging;
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
        Task<PagedList<OrderViewModel>> GetOrdersAsync(OrderStage orderStage, PagingParameters pagingParameters);

        Task<OrderViewModel> GetOrderAsync(Guid orderId);

        Task<OrderViewModel> GetOrderByDealIdAsync(Guid dealId);

        /// <summary>
        /// 获取用户订单在各个阶段的数量
        /// </summary>
        /// <returns></returns>
        Task<StageOrderCountViewModel> GetStageOrderCountAsync();
    }
}
