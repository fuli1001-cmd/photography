using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class Post : Entity, IAggregateRoot
    {
        public string Text { get; private set; }
        public double Timestamp { get; private set; }

        public int LikeCount { get; private set; }
        public int ShareCount { get; private set; }
        public int CommentCount { get; private set; }
        // for hot posts
        public int Score { get; private set; }
        public bool? Commentable { get; private set; }
        public ForwardType ForwardType { get; private set; }
        public ShareType ShareType { get; private set; }
        public Visibility Visibility { get; private set; }
        public string ViewPassword { get; private set; }
        public bool? ShowOriginalText { get; private set; }

        #region location properties
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public string LocationName { get; private set; }
        public string CityCode { get; private set; }
        #endregion

        public Post ForwardedPost { get; private set; }
        public Guid? ForwardedPostId { get; private set; }

        private readonly List<Post> _forwardingPosts = null;
        public IReadOnlyCollection<Post> ForwardingPosts => _forwardingPosts;

        private readonly List<PostAttachment> _postAttachments = null;
        public IReadOnlyCollection<PostAttachment> PostAttachments => _postAttachments;

        private readonly List<UserPostRelation> _userPostRelations = null;
        public IReadOnlyCollection<UserPostRelation> UserPostRelations => _userPostRelations;

        private readonly List<Comment> _comments = null;
        public IReadOnlyCollection<Comment> Comments => _comments;

        public User User { get; private set; }
        public Guid UserId { get; private set; }

        public Post()
        {

        }

        public Post(string text, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            double latitude, double longitude, string locationName, string cityCode, 
            List<Guid> friendIds, List<PostAttachment> postAttachments, Guid userId)
        {
            Text = text;
            Commentable = commentable;
            ForwardType = forwardType;
            ShareType = shareType;
            Visibility = visibility;
            ViewPassword = viewPassword;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            CityCode = cityCode;
            _postAttachments = postAttachments;
            _userPostRelations = friendIds?.Select(id => new UserPostRelation(id, UserPostRelationType.View)).ToList();
            UserId = userId;
            Timestamp = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
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
        Password,
        SelectedFriends
    }
}
