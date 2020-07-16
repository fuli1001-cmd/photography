using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.CommentAggregate
{
    public class Comment : Entity, IAggregateRoot
    {
        public string Text { get; private set; }
        public int Likes { get; private set; }
        public double CreatedTime { get; private set; }

        public PostAggregate.Post Post { get; private set; }
        public Guid PostId { get; private set; }

        public User User { get; private set; }
        public Guid UserId { get; private set; }

        public Comment ParentComment { get; private set; }
        public Guid? ParentCommentId { get; private set; }

        private readonly List<Comment> _subComments = null;
        public IReadOnlyCollection<Comment> SubComments => _subComments;

        private readonly List<UserCommentRelation> _userCommentRelations = null;
        public IReadOnlyCollection<UserCommentRelation> UserCommentRelations => _userCommentRelations;

        public Comment()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        //public Comment(string text, Guid? postId, Guid? commentId, Guid userId) : this()
        //{
        //    Text = text;
        //    PostId = postId;
        //    ParentCommentId = commentId;
        //    UserId = userId;
        //}

        public void ReplyPost(string text, Guid postId, Guid userId)
        {
            Text = text;
            PostId = postId;
            UserId = userId;
            AddRepliedPostDomainEvent();
        }

        public void ReplyComment(string text, Guid postId, Guid commentId, Guid userId)
        {
            Text = text;
            PostId = postId;
            ParentCommentId = commentId;
            UserId = userId;
            AddRepliedPostDomainEvent();
        }

        public void Like()
        {
            Likes++;
        }

        public void UnLike()
        {
            Likes = Math.Max(Likes - 1, 0);
        }

        public void Delete()
        {
            AddCommentDeletedDomainEvent();
        }

        private void AddRepliedPostDomainEvent()
        {
            var repliedPostDomainEvent = new RepliedPostDomainEvent(PostId);
            AddDomainEvent(repliedPostDomainEvent);
        }

        private void AddCommentDeletedDomainEvent()
        {
            // 删除的评论包括当前评论及其所有子评论
            var commentIds = _subComments.Select(c => c.Id).ToList();
            commentIds.Add(Id);
            var @event = new CommentDeletedDomainEvent(PostId, commentIds);
            AddDomainEvent(@event);
        }
    }
}
