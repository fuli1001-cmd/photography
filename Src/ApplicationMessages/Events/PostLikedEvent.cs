using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostLikedEvent : BaseEvent
    {
        // 点赞的用户id
        public Guid LikingUserId { get; set; }

        // 被赞帖子所属用户id
        public Guid PostUserId { get; set; }

        // 被赞的帖子id
        public Guid PostId { get; set; }
    }
}
