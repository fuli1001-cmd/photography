using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class UserFollowedEvent : BaseEvent
    {
        // 关注者id
        public Guid FollowerId { get; set; }

        // 被关注者id
        public Guid FollowedUserId { get; set; }
    }
}
