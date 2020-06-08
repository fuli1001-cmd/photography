using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.Events
{
    public class DeletedGroupDomainEvent : INotification
    {
        public Guid GroupId { get; }

        public DeletedGroupDomainEvent(Guid groupId)
        {
            GroupId = groupId;
        }
    }
}
