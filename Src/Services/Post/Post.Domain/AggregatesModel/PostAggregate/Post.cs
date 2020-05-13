using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class Post : Entity, IAggregateRoot
    {
        public string Text { get; private set; }
        public int LikeCount { get; private set; }
        public int ShareCount { get; private set; }
        public int CommentCount { get; private set; }
        // for hot posts
        public int Points { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool? Commentable { get; private set; }
        public ForwardType ForwardType { get; private set; }
        public ShareType ShareType { get; private set; }
        public Visibility Visibility { get; private set; }
        public string ViewPassword { get; private set; }
        public Location Location { get; private set; }

        public Post ForwardedPost { get; private set; }
        public Guid? ForwardedPostId { get; private set; }

        private readonly List<Post> _forwardingPosts;
        public IReadOnlyCollection<Post> ForwardingPosts => _forwardingPosts;

        private readonly List<PostAttachment> _postAttachments;
        public IReadOnlyCollection<PostAttachment> PostAttachments => _postAttachments;

        private readonly List<Comment> _comments;
        public IReadOnlyCollection<Comment> Comments => _comments;

        public User User { get; private set; }
        public Guid UserId { get; private set; }
    }

    public enum ForwardType
    {
        Allowed,
        Forbidden,
        Friends
    }

    public enum ShareType
    {
        Allowed,
        Forbidden,
        Friends
    }

    public enum Visibility
    {
        Public,
        Friends,
        SelectedFriends,
        Password
    }
}
