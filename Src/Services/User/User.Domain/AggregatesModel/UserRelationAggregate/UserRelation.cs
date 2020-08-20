using System;
using System.Collections.Generic;
using System.Text;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.Events;

namespace Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate
{
    /// <summary>
    /// 用户之间的关系（关注，静音），关系从FromUser指向ToUser
    /// </summary>
    public class UserRelation : Entity, IAggregateRoot
    {
        public Guid FromUserId { get; private set; }
        //public UserAggregate.User FromUser { get; private set; }

        public Guid ToUserId { get; private set; }
        //public UserAggregate.User ToUser { get; private set; }

        // FromUser是否静音了ToUser
        public bool Muted { get; private set; }

        // FromUser是否关注了ToUser
        public bool Followed { get; private set; }

        // 关注时间
        public DateTime? FollowTime { get; private set; }

        public UserRelation(Guid fromUserId, Guid toUserId)
        {
            FromUserId = fromUserId;
            ToUserId = toUserId;
        }

        public void Mute()
        {
            Muted = true;
        }

        public void UnMute()
        {
            Muted = false;
        }

        public void Follow()
        {
            Followed = true;
            FollowTime = DateTime.UtcNow;
            AddFollowedUserDomainEvent();
        }

        public void UnFollow()
        {
            Followed = false;
            FollowTime = null;
            AddUnFollowedUserDomainEvent();
        }

        private void AddFollowedUserDomainEvent()
        {
            var followedUserDomainEvent = new FollowedUserDomainEvent(FromUserId, ToUserId);
            AddDomainEvent(followedUserDomainEvent);
        }

        private void AddUnFollowedUserDomainEvent()
        {
            var unFollowedUserDomainEvent = new UnFollowedUserDomainEvent(FromUserId, ToUserId);
            AddDomainEvent(unFollowedUserDomainEvent);
        }
    }
}
