using System;
using System.Collections.Generic;
using System.Text;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.Events;

namespace Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate
{
    public class UserRelation : Entity, IAggregateRoot
    {
        // 关注者
        public Guid FollowerId { get; private set; }
        public Domain.AggregatesModel.UserAggregate.User Follower { get; private set; }

        // 被关注者
        public Guid FollowedUserId { get; private set; }
        public Domain.AggregatesModel.UserAggregate.User FollowedUser { get; private set; }

        public bool MutedFollowedUser { get; private set; }

        public UserRelation()
        {
            MutedFollowedUser = false;
        }

        public UserRelation(Guid followerId, Guid followedUserId) : this()
        {
            FollowerId = followerId;
            FollowedUserId = followedUserId;
        }

        public void Follow()
        {
            AddFollowedUserDomainEvent();
        }

        public void UnFollow()
        {
            AddUnFollowedUserDomainEvent();
        }

        private void AddFollowedUserDomainEvent()
        {
            var followedUserDomainEvent = new FollowedUserDomainEvent(FollowerId, FollowedUserId);
            AddDomainEvent(followedUserDomainEvent);
        }

        private void AddUnFollowedUserDomainEvent()
        {
            var unFollowedUserDomainEvent = new UnFollowedUserDomainEvent(FollowerId, FollowedUserId);
            AddDomainEvent(unFollowedUserDomainEvent);
        }
    }
}
