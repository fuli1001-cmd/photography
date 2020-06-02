using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.EventAggregate
{
    public interface IEventRepository : IRepository<Event>
    {
    }
}
