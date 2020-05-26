using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class RepliedPostDomainEvent : INotification
    {
        public Guid PostId { get; }

        public RepliedPostDomainEvent(Guid postId)
        {
            PostId = postId;
        }
    }
}
