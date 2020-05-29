using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostDeletedEvent : BaseEvent
    {
        public Guid UserId { get; set; }
    }
}
