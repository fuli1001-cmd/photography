using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class OrderRejectedEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public Guid DealId { get; set; }
    }
}
