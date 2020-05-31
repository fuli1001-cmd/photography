using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class UserUnFollowedEvent : BaseEvent
    {
        // 取消关注者id
        public Guid FollowerId { get; set; }

        // 被取消关注者id
        public Guid FollowedUserId { get; set; }
    }
}
