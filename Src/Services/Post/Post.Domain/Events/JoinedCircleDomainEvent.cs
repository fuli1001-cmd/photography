﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class JoinedCircleDomainEvent : INotification
    {
        public Guid CircleId { get; }

        public JoinedCircleDomainEvent(Guid circleId)
        {
            CircleId = circleId;
        }
    }
}
