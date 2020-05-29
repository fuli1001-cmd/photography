using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostPublishedEvent : BaseEvent
    {
        public Guid UserId { get; set; }
    }
}
