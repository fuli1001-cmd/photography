using Arise.DDD.API.Paging;
using Photography.Services.Notification.API.Query.ViewModels;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.API.Query.Interfaces
{
    public interface IEventQueries
    {
        ///// <summary>
        ///// 获取用户所有事件
        ///// </summary>
        ///// <param name="pagingParameters"></param>
        ///// <returns></returns>
        //Task<PagedList<EventViewModel>> GetUserReceivedEventsAsync(PagingParameters pagingParameters);

        /// <summary>
        /// 按类别获取用户的事件
        /// </summary>
        /// <param name="eventCategory">事件类别</param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        Task<PagedList<EventViewModel>> GetUserCategoryEventsAsync(EventCategory eventCategory, PagingParameters pagingParameters);

        /// <summary>
        /// 获取用户各事件类别的未读数量
        /// </summary>
        /// <returns></returns>
        Task<UnReadEventCountViewModel> GetUnReadEventCountAsync();
    }
}
