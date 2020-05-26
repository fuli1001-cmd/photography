using Arise.DDD.Domain.SeedWork;
using Microsoft.VisualBasic.CompilerServices;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Events;
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

        public UserPostRelation(string userId, Guid postId)
        {
            UserId = Guid.Parse(userId);
            PostId = postId;
        }

        public UserPostRelation(Guid userId, UserPostRelationType userPostRelationType)
        {
            UserId = userId;
            UserPostRelationType = userPostRelationType;
        }

        public void Like()
        {
            UserPostRelationType = UserPostRelationType.Like;
            AddUserLikedPostDomainEvent();
        }

        public void UnLike()
        {
            AddUserUnLikedPostDomainEvent();
        }

        private void AddUserLikedPostDomainEvent()
        {
            var userLikedPostDomainEvent = new UserLikedPostDomainEvent(PostId);
            AddDomainEvent(userLikedPostDomainEvent);
        }

        private void AddUserUnLikedPostDomainEvent()
        {
            var userUnLikedPostDomainEvent = new UserUnLikedPostDomainEvent(PostId);
            AddDomainEvent(userUnLikedPostDomainEvent);
        }
    }

    public enum UserPostRelationType
    {
        View,
        Like,
        Share
    }
}
