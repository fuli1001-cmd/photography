using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.User
{
    public class UserFeedbackCreatedEvent : BaseEvent
    {
        public Guid UserId { get; set; }
    }
}
