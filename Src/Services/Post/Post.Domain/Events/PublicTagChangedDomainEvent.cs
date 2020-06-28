using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class PublicTagChangedDomainEvent : INotification
    {
        public List<string> AppliedTags { get; }

        public List<string> RemovedTags { get; }

        public PublicTagChangedDomainEvent(List<string> appliedTags, List<string> removedTags)
        {
            AppliedTags = appliedTags;
            RemovedTags = removedTags;
        }
    }
}
