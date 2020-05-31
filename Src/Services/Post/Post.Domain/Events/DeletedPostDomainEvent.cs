using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class DeletedPostDomainEvent : INotification
    {
        public Guid PostId { get; }
        public List<Guid> CommentIds { get; }

        public DeletedPostDomainEvent(Guid postId, List<Guid> commentIds)
        {
            PostId = postId;
            CommentIds = commentIds;
        }
    }
}
