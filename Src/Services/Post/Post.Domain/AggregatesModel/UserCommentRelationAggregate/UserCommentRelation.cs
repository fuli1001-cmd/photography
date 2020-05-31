using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate
{
    /// <summary>
    /// 用户与评论的关系（目前只表示用户点赞评论）
    /// 一个用户可以点赞多个评论
    /// 一个评论可以被多个用户点赞
    /// </summary>
    public class UserCommentRelation : Entity, IAggregateRoot
    {
        public Guid CommentId { get; private set; }
        public CommentAggregate.Comment Comment { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public UserCommentRelation() { }

        public UserCommentRelation(Guid userId, Guid commentId)
        {
            UserId = userId;
            CommentId = commentId;
        }

        public void Like(Guid postId)
        {
            AddUserLikedCommentDomainEvent(postId);
        }

        public void UnLike(Guid postId)
        {
            AddUserUnLikedCommentDomainEvent(postId);
        }

        private void AddUserLikedCommentDomainEvent(Guid postId)
        {
            var userLikedCommentDomainEvent = new UserLikedCommentDomainEvent(postId);
            AddDomainEvent(userLikedCommentDomainEvent);
        }

        private void AddUserUnLikedCommentDomainEvent(Guid postId)
        {
            var userUnLikedCommentDomainEvent = new UserUnLikedCommentDomainEvent(postId);
            AddDomainEvent(userUnLikedCommentDomainEvent);
        }
    }
}
