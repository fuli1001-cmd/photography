using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class PrivateTagSharedDomainEvent : INotification
    {
        public string PrivateTag { get; }

        public PrivateTagSharedDomainEvent(string privateTag)
        {
            PrivateTag = privateTag;
        }
    }
}
