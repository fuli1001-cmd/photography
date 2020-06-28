using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class PrivateTagDeletedDomainEvent : INotification
    {
        public Guid UserId { get; }

        public string Name { get; }

        public PrivateTagDeletedDomainEvent(Guid userId, string name)
        {
            UserId = userId;
            Name = name;
        }
    }
}
