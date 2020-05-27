using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Messages.Events
{
    public class OrderCanceledEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public Guid DealId { get; set; }
    }
}
