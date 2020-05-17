using System;
using System.Collections.Generic;
using System.Text;
using Arise.DDD.Domain.SeedWork;

namespace Photography.Services.User.Domain.AggregatesModel.UserAggregate
{
    public class UserRelation : Entity
    {
        // 关注者
        public Guid FollowerId { get; private set; }
        public User Follower { get; private set; }

        // 被关注者
        public Guid FollowedUserId { get; private set; }
        public User FollowedUser { get; private set; }
    }
}
