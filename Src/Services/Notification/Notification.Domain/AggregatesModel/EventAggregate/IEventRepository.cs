using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.Domain.AggregatesModel.EventAggregate
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<List<Event>> GetUnProcessedEventsAsync(Guid fromUserId, Guid toUserId, EventType eventType);

        /// <summary>
        /// 获取用户某个类别下的所有未读通知
        /// </summary>
        /// <param name="toUserId">用户id</param>
        /// <param name="eventCategory">通知类别</param>
        /// <returns></returns>
        Task<List<Event>> GetUserUnReadCategoryEventsAsync(Guid toUserId, EventCategory eventCategory);

        /// <summary>
        /// 获取用户某个类别下的所有通知
        /// </summary>
        /// <param name="toUserId">用户id</param>
        /// <param name="eventCategory">通知类别</param>
        /// <returns></returns>
        Task<List<Event>> GetUserCategoryEventsAsync(Guid toUserId, EventCategory eventCategory);

        /// <summary>
        /// 根据通知id数组获取通知
        /// </summary>
        /// <param name="eventIds">通知id数组</param>
        /// <returns></returns>
        Task<List<Event>> GetEventsAsync(List<Guid> eventIds);
    }
}
