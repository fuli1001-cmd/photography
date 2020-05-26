using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Messages.Events
{
    public class OrderAcceptedEvent : BaseEvent
    {
        public Guid DealId { get; set; }
    }
}
