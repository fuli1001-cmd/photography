using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class EventRepository : EfRepository<Event, NotificationContext>, IEventRepository
    {
        public EventRepository(NotificationContext context) : base(context)
        {

        }

        public Task<List<Event>> GetUnProcessedEventsAsync(Guid fromUserId, Guid toUserId, EventType eventType)
        {
            return _context.Events.Where(e => e.FromUserId == fromUserId && e.ToUserId == toUserId && e.EventType == eventType).ToListAsync();
        }
    }
}
