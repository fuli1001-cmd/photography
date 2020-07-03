using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class PostSharedDomainEvent : INotification
    {
        public Guid PostId { get; }

        public PostSharedDomainEvent(Guid postId)
        {
            PostId = postId;
        }
    }
}
