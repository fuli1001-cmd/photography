using Arise.DDD.Domain.SeedWork;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Nickname { get; private set; }
        public string Avatar { get; private set; }

        private readonly List<Event> _raisedEvents = null;
        public IReadOnlyCollection<Event> RaisedEvents => _raisedEvents;

        private readonly List<Event> _receivedEvents = null;
        public IReadOnlyCollection<Event> ReceivedEvents => _receivedEvents;

        public List<UserRelation> Followers { get; private set; }
        public List<UserRelation> FollowedUsers { get; private set; }

        public User() { }

        public User(Guid id, string nickName)
        {
            Id = id;
            Nickname = nickName;
        }

        public void Update(string nickName, string avatar)
        {
            Nickname = nickName;
            Avatar = avatar;
        }
    }
}
