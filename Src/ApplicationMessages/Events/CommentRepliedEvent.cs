using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class CommentRepliedEvent : BaseEvent
    {
        // 被回复的评论id
        public Guid CommentId { get; set; }

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
