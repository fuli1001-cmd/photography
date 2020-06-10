using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate
{
    public class GroupUser : Entity, IAggregateRoot
    {
        public Guid? GroupId { get; private set; }
        public Group Group { get; private set; }

        public Guid? UserId { get; private set; }
        public UserAggregate.User User { get; private set; }

        public bool Muted { get; private set; }

        public GroupUser() { }

        public GroupUser(Guid userId) 
        {
            UserId = userId;
        }

        public GroupUser(Guid groupId, Guid userId) : this(userId)
        {
            GroupId = groupId;
        }

        public void Mute()
        {
            Muted = true;
        }

        public void UnMute()
        {
            Muted = false;
        }
    }
}
