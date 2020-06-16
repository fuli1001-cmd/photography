using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class UserUnLikedCommentDomainEvent : INotification
    {
        public Guid PostId { get; }

        public Guid CommentId { get; }

        public UserUnLikedCommentDomainEvent(Guid postId, Guid commentId)
        {
            PostId = postId;
            CommentId = commentId;
        }
    }
}
