using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class CommentDeletedDomainEvent : INotification
    {
        /// <summary>
        /// 被删除评论所属的帖子id
        /// </summary>
        public Guid PostId { get; }

        /// <summary>
        /// 被删除的评论id及其子评论id
        /// </summary>
        public List<Guid> CommentIds { get; }

        public CommentDeletedDomainEvent(Guid postId, List<Guid> commentIds)
        {
            PostId = postId;
            CommentIds = commentIds;
        }
    }
}
