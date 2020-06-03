using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class CommentLikedEvent : BaseEvent
    {
        // 点赞的用户id
        public Guid LikingUserId { get; set; }

        // 被赞评论所属帖子所属用户id
        public Guid PostUserId { get; set; }

        // 被赞评论所属的帖子id
        public Guid PostId { get; set; }

        // 被赞评论所属用户id
        public Guid CommentUserId { get; set; }

        // 被赞的评论id
        public Guid CommentId { get; set; }

        // 被赞评论的内容
        public string CommentText { get; set; }
    }
}
