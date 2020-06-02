using Arise.DDD.Domain.SeedWork;
using Microsoft.VisualBasic.CompilerServices;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate
{
    /// <summary>
    /// 用户与帖子的关系（查看，赞）
    /// 一个用户可以点赞或参看多个帖子
    /// 一个帖子可以被多个用户点赞或查看
    /// </summary>
    public class UserPostRelation : Entity, IAggregateRoot
    {
        public Guid? PostId { get; private set; }
        public PostAggregate.Post Post { get; private set; }

        public Guid? UserId { get; private set; }
        public User User { get; private set; }

        public UserPostRelationType UserPostRelationType { get; set; }

        public UserPostRelation() { }

        public UserPostRelation(Guid userId, Guid postId)
        {
            UserId = userId;
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
            var userLikedPostDomainEvent = new UserLikedPostDomainEvent(PostId.Value);
            AddDomainEvent(userLikedPostDomainEvent);
        }

        private void AddUserUnLikedPostDomainEvent()
        {
            var userUnLikedPostDomainEvent = new UserUnLikedPostDomainEvent(PostId.Value);
            AddDomainEvent(userUnLikedPostDomainEvent);
        }
    }

    public enum UserPostRelationType
    {
        View,
        Like
    }
}
