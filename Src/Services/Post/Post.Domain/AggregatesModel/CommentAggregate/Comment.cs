using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.CommentAggregate
{
    public class Comment : Entity, IAggregateRoot
    {
        public string Text { get; private set; }
        public int Likes { get; private set; }
        public double CreatedTime { get; private set; }

        public PostAggregate.Post Post { get; private set; }
        public Guid? PostId { get; private set; }

        public User User { get; private set; }
        public Guid UserId { get; private set; }

        public Comment ParentComment { get; private set; }
        public Guid? ParentCommentId { get; private set; }

        private readonly List<Comment> _subComments = null;
        public IReadOnlyCollection<Comment> SubComments => _subComments;

        public Comment()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public Comment(string text, Guid? postId, Guid? commentId, Guid userId) : this()
        {
            Text = text;
            PostId = postId;
            ParentCommentId = commentId;
            UserId = userId;
        }
    }
}
