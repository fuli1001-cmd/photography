using Arise.DDD.Domain.SeedWork;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate
{
    /// <summary>
    /// 用户之间的关系（关注与被关注）
    /// 一个用户可以关注多个用户
    /// 一个用户可以被多个用户关注
    /// </summary>
    public class UserRelation : Entity, IAggregateRoot
    {
        // 关注者
        public Guid FollowerId { get; private set; }
        public User Follower { get; private set; }

        // 被关注者
        public Guid FollowedUserId { get; private set; }
        public User FollowedUser { get; private set; }

        public UserRelation() { }

        public UserRelation(Guid followerId, Guid followedUserId) : this()
        {
            FollowerId = followerId;
            FollowedUserId = followedUserId;
        }
    }
}
