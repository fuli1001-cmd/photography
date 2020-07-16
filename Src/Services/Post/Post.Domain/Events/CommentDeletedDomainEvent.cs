using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class CommentDeletedDomainEvent : INotification
    {
        public List<Guid> CommentIds { get; }

        public CommentDeletedDomainEvent(List<Guid> commentIds)
        {
            CommentIds = commentIds;
        }
    }
}
