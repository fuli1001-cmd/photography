using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class UserUnLikedCommentDomainEvent : INotification
    {
        public Guid PostId { get; }

        public UserUnLikedCommentDomainEvent(Guid postId)
        {
            PostId = postId;
        }
    }
}
