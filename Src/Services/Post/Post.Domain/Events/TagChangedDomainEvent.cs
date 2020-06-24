using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.Events
{
    public class TagChangedDomainEvent : INotification
    {
        public List<string> AppliedTags { get; }

        public List<string> RemovedTags { get; }

        public TagChangedDomainEvent(List<string> appliedTags, List<string> removedTags)
        {
            AppliedTags = appliedTags;
            RemovedTags = removedTags;
        }
    }
}
