using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostRepliedEvent : BaseEvent
    {
        // 评论人id
        public Guid FromUserId { get; set; }

        // 被回复的人的id
        public Guid ToUserId { get; set; }

        // 被回复的帖子id
        public Guid PostId { get; set; }

        // 回复内容
        public string Text { get; set; }
    }
}
