using Arise.DDD.Domain.SeedWork;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.PostAggregate
{
    public class Post : Entity, IAggregateRoot
    {
        public string Image { get; private set; }

        private readonly List<Event> _events = null;
        public IReadOnlyCollection<Event> Events => _events;

        public Post() { }

        public Post(Guid id, string image)
        {
            Id = id;
            Image = image;
        }
    }
}
