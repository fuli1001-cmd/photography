using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Messages.Events
{
    public class AppointmentDealAcceptedEvent : BaseEvent
    {
        public Guid DealId { get; set; }
    }
}
