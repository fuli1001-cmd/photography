using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.Queries
{
    public class EventCategoryTypeHelper
    {
        public static List<EventType> GetEventCategoryTypes(EventCategory eventCategory)
        {
            var types = new List<EventType>();

            if (eventCategory == EventCategory.Interaction)
            {
                types = new List<EventType>
                {
                    EventType.ReplyPost,
                    EventType.LikePost,
                    EventType.ForwardPost,
                    EventType.SharePost,
                    EventType.ReplyComment,
                    EventType.LikeComment,
                    EventType.Follow,
                    EventType.ApplyJoinCircle,
                    EventType.JoinCircle,
                    EventType.CircleOwnerChanged
                };
            }
            else if (eventCategory == EventCategory.Appointment)
            {
                types = new List<EventType>
                {
                    EventType.CancelOrder,
                    EventType.RejectOrder
                };
            }
            else if (eventCategory == EventCategory.System)
            {
                types = new List<EventType>
                {
                    EventType.DeletePost,
                    EventType.IdAuthenticated,
                    EventType.IdRejected
                };
            }

            return types;
        }
    }
}
