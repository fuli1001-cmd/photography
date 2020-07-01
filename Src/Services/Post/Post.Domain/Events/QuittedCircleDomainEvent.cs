using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class QuittedCircleDomainEvent : INotification
    {
        public Guid CircleId { get; }

        public QuittedCircleDomainEvent(Guid circleId)
        {
            CircleId = circleId;
        }
    }
}
