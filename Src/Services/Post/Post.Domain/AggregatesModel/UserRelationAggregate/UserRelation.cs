using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate
{
    public class UserRelation : Entity, IAggregateRoot
    {
        // 关注者
        public Guid FollowerId { get; private set; }
        public User Follower { get; private set; }

        // 被关注者
        public Guid FollowedUserId { get; private set; }
        public User FollowedUser { get; private set; }

        public UserRelation()
        {
            
        }

        public UserRelation(Guid followerId, Guid followedUserId) : this()
        {
            FollowerId = followerId;
            FollowedUserId = followedUserId;
        }
    }
}
