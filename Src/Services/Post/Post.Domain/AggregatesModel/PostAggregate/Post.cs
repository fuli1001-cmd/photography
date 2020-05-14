using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int Score { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool? Commentable { get; private set; }
        public ForwardType ForwardType { get; private set; }
        public ShareType ShareType { get; private set; }
        public Visibility Visibility { get; private set; }
        public string ViewPassword { get; private set; }

        public string Province { get; private set; }
        public string City { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public string LocationName { get; private set; }
        public string Address { get; private set; }

        public Post ForwardedPost { get; private set; }
        public Guid? ForwardedPostId { get; private set; }

        private readonly List<Post> _forwardingPosts = null;
        public IReadOnlyCollection<Post> ForwardingPosts => _forwardingPosts;

        private readonly List<PostAttachment> _postAttachments = null;
        public IReadOnlyCollection<PostAttachment> PostAttachments => _postAttachments;

        private readonly List<PostForUser> _postForUsers = null;
        public IReadOnlyCollection<PostForUser> PostForUsers => _postForUsers;

        private readonly List<Comment> _comments = null;
        public IReadOnlyCollection<Comment> Comments => _comments;

        public User User { get; private set; }
        public Guid UserId { get; private set; }

        public Post()
        {

        }

        public Post(string text, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            string province, string city, double latitude, double longitude, string locationName, string address, 
            List<Guid> friendIds, List<PostAttachment> postAttachments)
        {
            Text = text;
            Commentable = commentable;
            ForwardType = forwardType;
            ShareType = shareType;
            Visibility = visibility;
            ViewPassword = viewPassword;
            Province = province;
            City = city;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            Address = address;
            _postAttachments = postAttachments;
            _postForUsers = friendIds.Select(id => new PostForUser(id)).ToList();
        }
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
