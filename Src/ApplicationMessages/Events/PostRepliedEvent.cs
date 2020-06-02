using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostRepliedEvent : BaseEvent
    {
        // 评论人id
        public Guid ReplyUserId { get; set; }

        public string ReplyUserNickname { get; set; }

        public string ReplyUserAvatar { get; set; }

        // 被回复的帖子所有人id
        public Guid PostUserId { get; set; }

        // 被回复的帖子id
        public Guid PostId { get; set; }

        // 被回复的帖子图片
        public string PostImage { get; set; }
    }
}
