using Arise.DDD.Infrastructure;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class EventRepository : EfRepository<Event, NotificationContext>, IEventRepository
    {
        public EventRepository(NotificationContext context) : base(context)
        {

        }
    }
}
