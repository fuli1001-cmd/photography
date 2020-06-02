using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostUnLikedEvent : BaseEvent
    {
        // 被取消赞帖子所属用户id
        public Guid PostUserId { get; set; }

        // 取消点赞的用户id
        public Guid LikingUserId { get; set; }
    }
}
