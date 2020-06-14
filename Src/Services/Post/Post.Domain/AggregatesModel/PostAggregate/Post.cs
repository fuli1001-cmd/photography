using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class Post : Entity, IAggregateRoot
    {
        public string Text { get; private set; }
        public double CreatedTime { get; private set; }
        public double? UpdatedTime { get; private set; }
        public PostType PostType { get; private set; }


        public User User { get; private set; }
        public Guid UserId { get; private set; }

        private readonly List<PostAttachment> _postAttachments = null;
        public IReadOnlyCollection<PostAttachment> PostAttachments => _postAttachments;

        #region location properties
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public string LocationName { get; private set; }
        public string Address { get; private set; }
        public string CityCode { get; private set; }
        #endregion

        #region post only properties
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

        public Post ForwardedPost { get; private set; }
        public Guid? ForwardedPostId { get; private set; }

        private readonly List<Post> _forwardingPosts = null;
        public IReadOnlyCollection<Post> ForwardingPosts => _forwardingPosts;

        private readonly List<UserPostRelation> _userPostRelations = null;
        public IReadOnlyCollection<UserPostRelation> UserPostRelations => _userPostRelations;

        private readonly List<Comment> _comments = null;
        public IReadOnlyCollection<Comment> Comments => _comments;
        #endregion

        #region appointment only properties
        public double? AppointedTime { get; private set; }
        public decimal? Price { get; private set; }
        public PayerType? PayerType { get; private set; }
        public AppointmentDealStatus? AppointmentDealStatus { get; private set; }

        public User AppointmentedUser { get; private set; }
        public Guid? AppointmentedUserId { get; private set; }

        public Post AppointmentedToPost { get; private set; }
        public Guid? AppointmentedToPostId { get; private set; }

        private readonly List<Post> _appointmentedFromPosts = null;
        public IReadOnlyCollection<Post> AppointmentedFromPosts => _appointmentedFromPosts;
        #endregion

        public Post()
        {

        }

        // constructor to set common properties
        private Post(string text, double? latitude, double? longitude, string? locationName, string address, 
            string cityCode, List<PostAttachment> postAttachments, Guid userId)
        {
            Text = text;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            Address = address;
            CityCode = cityCode;
            _postAttachments = postAttachments;
            UserId = userId;
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 构造帖子对象
        private Post(string text, bool? showOriginalText, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            double latitude, double longitude, string locationName, string address, string cityCode, 
            List<Guid> friendIds, List<PostAttachment> postAttachments, Guid userId)
            : this(text, latitude, longitude, locationName, address, cityCode, postAttachments, userId)
        {
            Commentable = commentable;
            ForwardType = forwardType;
            ShareType = shareType;
            Visibility = visibility;
            ViewPassword = viewPassword;
            _userPostRelations = friendIds?.Select(id => new UserPostRelation(id, UserPostRelationType.View)).ToList();
            PostType = PostType.Post;
            ShowOriginalText = showOriginalText;
        }

        // 构造约拍对象
        private Post(string text, double? appointedTime, decimal? price, PayerType? payerType, double? latitude, double? longitude, 
            string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId)
            : this(text, latitude, longitude, locationName, address, cityCode, postAttachments, userId)
        {
            AppointedTime = appointedTime;
            Price = price;
            PayerType = payerType;
            PostType = PostType.Appointment;
        }

        // 构造关联约拍对象
        private Post(string text, double? appointedTime, decimal? price, PayerType? payerType, double? latitude, double? longitude,
            string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId, 
            Guid appointmentedUserId, Guid? appointmentedToPostId)
            : this(text, appointedTime, price, payerType, latitude, longitude, locationName, address, cityCode, postAttachments, userId)
        {
            AppointmentedUserId = appointmentedUserId;
            AppointmentedToPostId = appointmentedToPostId;
            PostType = PostType.AppointmentDeal;
            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Created;
        }

        // 创建帖子对象
        public static Post CreatePost(string text, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            double latitude, double longitude, string locationName, string address, string cityCode,
            List<Guid> friendIds, List<PostAttachment> postAttachments, Guid userId, bool? showOriginalText = null)
        {
            return new Post(text, showOriginalText, commentable, forwardType, shareType, visibility, viewPassword, latitude, longitude,
                locationName, address, cityCode, friendIds, postAttachments, userId);
        }

        // 创建约拍对象
        public static Post CreateAppointment(string text, double? appointedTime, decimal? price, PayerType payerType, double? latitude, double? longitude,
            string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId)
        {
            return new Post(text, appointedTime, price, payerType, latitude, longitude, locationName, address, cityCode, postAttachments, userId);
        }

        // 创建约拍交易对象
        public static Post CreateAppointmentDeal(string text, double? appointedTime, decimal? price, PayerType? payerType, double? latitude, double? longitude,
            string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId, Guid appointmentedUserId, Guid? appointmentedToPostId)
        {
            return new Post(text, appointedTime, price, payerType, latitude, longitude, locationName, address, cityCode, postAttachments, userId, appointmentedUserId, appointmentedToPostId);
        }

        // 更新帖子对象
        public void Update(string text, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            double latitude, double longitude, string locationName, string address, string cityCode,
            List<Guid> friendIds, List<PostAttachment> postAttachments)
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
            Address = address;
            CityCode = cityCode;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

            var relations = friendIds?.Select(id => new UserPostRelation(id, UserPostRelationType.View)).ToList() ?? new List<UserPostRelation>();
            for (var i = 0; i < _userPostRelations.Count; i++)
            {
                if (_userPostRelations[i].UserPostRelationType == UserPostRelationType.View)
                {
                    _userPostRelations.RemoveAt(i);
                    i--;
                }
            }
            _userPostRelations.AddRange(relations);

            _postAttachments.Clear();
            _postAttachments.AddRange(postAttachments ?? new List<PostAttachment>());
        }

        public void Delete()
        {
            if (PostType == PostType.Post)
            {
                AddDeletedPostDomainEvent();
                _comments.Clear();
                _forwardingPosts.Clear();
            }
            else if (PostType == PostType.Appointment)
            {
                _appointmentedFromPosts.Clear();
            }

            _postAttachments.Clear();
        }

        public void SetForwardPostId(Guid forwardedPostId)
        {
            ForwardedPostId = forwardedPostId;
        }

        public void CancelAppointmentDeal(Guid userId)
        {
            //  当前操作用户必须为发出该交易的用户
            if (UserId != userId)
                throw new ClientException("操作失败。", new List<string> { "Appointment deal is not created by current user." });

            if (AppointmentDealStatus != PostAggregate.AppointmentDealStatus.Created)
                throw new ClientException("操作失败。", new List<string> { "Current appointment deal status is not 'Created'." });

            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Canceled;
        }

        public void AcceptAppointmentDeal(Guid userId)
        {
            //  当前操作用户必须为收到该交易的用户
            if (AppointmentedUserId != userId)
                throw new ClientException("操作失败。", new List<string> { "Appointmented user is not current user." });

            if (AppointmentDealStatus != PostAggregate.AppointmentDealStatus.Created)
                throw new ClientException("操作失败。", new List<string> { "Current appointment deal status is not 'Created'." });

            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Accepted;
        }

        public void RejectAppointmentDeal(Guid userId)
        {
            // 当前操作用户必须为收到该交易的用户
            if (AppointmentedUserId != userId)
                throw new ClientException("操作失败。", new List<string> { "Appointmented user is not current user." });

            if (AppointmentDealStatus != PostAggregate.AppointmentDealStatus.Created)
                throw new ClientException("操作失败。", new List<string> { "Current appointment deal status is not 'Created'." });

            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Rejected;
        }

        public void Like()
        {
            LikeCount++;
        }

        public void UnLike()
        {
            LikeCount = Math.Max(LikeCount - 1, 0);
        }

        public void Share()
        {
            ShareCount++;
        }

        public void Comment()
        {
            if (Commentable != null && Commentable.Value)
                CommentCount++;
        }

        private void AddDeletedPostDomainEvent()
        {
            var commentIds = _comments.Select(c => c.Id).ToList();
            var deletedPostDomainEvent = new DeletedPostDomainEvent(Id, commentIds);
            AddDomainEvent(deletedPostDomainEvent);
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

    public enum PostType
    {
        Post,
        Appointment,
        AppointmentDeal
    }

    public enum PayerType
    {
        Free,
        Me,
        You
    }

    // 约拍生命流程状态
    public enum AppointmentDealStatus
    {
        // 已创建
        Created,
        // 已接受
        Accepted,
        // 已取消
        Canceled,
        // 已拒绝
        Rejected
    }
}
