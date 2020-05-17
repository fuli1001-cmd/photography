using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate
{
    public class UserPostRelation : Entity, IAggregateRoot
    {
        public Guid PostId { get; private set; }
        public PostAggregate.Post Post { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public UserPostRelationType UserPostRelationType { get; set; }

        public UserPostRelation(Guid userId, UserPostRelationType userPostRelationType)
        {
            UserId = userId;
            UserPostRelationType = userPostRelationType;
        }

        public UserPostRelation(Guid userId, Guid postId, UserPostRelationType userPostRelationType) : this(userId, userPostRelationType)
        {
            PostId = postId;
        }
    }

    public enum UserPostRelationType
    {
        View,
        Like
    }
}
