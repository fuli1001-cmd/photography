using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Messages.Events
{
    public class AppointmentDealRejectedEvent : BaseEvent
    {
        public Guid DealId { get; set; }
    }
}
