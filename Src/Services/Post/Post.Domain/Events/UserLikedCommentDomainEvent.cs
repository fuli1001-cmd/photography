using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class UserLikedCommentDomainEvent : INotification
    {
        public Guid PostId { get; }

        public UserLikedCommentDomainEvent(Guid postId)
        {
            PostId = postId;
        }
    }
}
