using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class CommentRepliedEvent : PostRepliedEvent
    {
        // 被回复的评论id
        public Guid CommentId { get; set; }
    }
}
