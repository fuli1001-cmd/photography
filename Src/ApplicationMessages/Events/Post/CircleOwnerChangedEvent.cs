using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Post
{
    public class CircleOwnerChangedEvent : BaseEvent
    {
        public Guid OldOwnerId { get; set; }

        public Guid NewOwnerId { get; set; }

        public Guid CircleId { get; set; }

        public string CircleName { get; set; }
    }
}
