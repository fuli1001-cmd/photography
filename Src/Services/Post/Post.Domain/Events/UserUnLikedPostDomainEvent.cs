using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class UserUnLikedPostDomainEvent : INotification
    {
        public Guid PostId { get; }

        public UserUnLikedPostDomainEvent(Guid postId)
        {
            PostId = postId;
        }
    }
}
