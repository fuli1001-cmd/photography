using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class CircleDeletedDomainEvent : INotification
    {
        public Guid CircleId { get; }

        public CircleDeletedDomainEvent(Guid circleId)
        {
            CircleId = circleId;
        }
    }
}
