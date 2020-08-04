using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Infrastructure.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class EventRepository : EfRepository<Event, NotificationContext>, IEventRepository
    {
        public EventRepository(NotificationContext context) : base(context) { }

        public async Task<List<Event>> GetEventsAsync(List<Guid> eventIds)
        {
            return await _context.Events.Where(e => eventIds.Contains(e.Id)).ToListAsync();
        }

        public async Task<List<Event>> GetUnProcessedEventsAsync(Guid fromUserId, Guid toUserId, EventType eventType)
        {
            return await _context.Events.Where(e => e.FromUserId == fromUserId && e.ToUserId == toUserId && e.EventType == eventType).ToListAsync();
        }

        public async Task<List<Event>> GetUserCategoryEventsAsync(Guid toUserId, EventCategory eventCategory)
        {
            var eventTypes = EventCategoryTypeHelper.GetEventCategoryTypes(eventCategory);

            return await _context.Events.Where(e => e.ToUserId == toUserId && eventTypes.Contains(e.EventType)).ToListAsync();
        }

        public async Task<List<Event>> GetUserUnReadCategoryEventsAsync(Guid toUserId, EventCategory eventCategory)
        {
            var eventTypes = EventCategoryTypeHelper.GetEventCategoryTypes(eventCategory);
            
            return await _context.Events.Where(e => e.ToUserId == toUserId && !e.Readed && eventTypes.Contains(e.EventType)).ToListAsync();
        }
    }
}
