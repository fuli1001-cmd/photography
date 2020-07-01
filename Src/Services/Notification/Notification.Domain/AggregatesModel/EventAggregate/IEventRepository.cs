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
    }
}
