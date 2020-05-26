using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class UserLikedPostDomainEvent : INotification
    {
        public Guid PostId { get; }

        public UserLikedPostDomainEvent(Guid postId)
        {
            PostId = postId;
        }
    }
}
