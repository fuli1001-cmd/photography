﻿using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
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
        public double UpdatedTime { get; private set; }
        public PostType PostType { get; private set; }

        // 是否已审核
        public PostAuthStatus PostAuthStatus { get; private set; }

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
        public int ForwardCount { get; private set; }
        // 帖子积分
        public int Score { get; private set; }
        // 帖子积分最后更新时间
        public double? LastScoreRefreshedTime { get; private set; } 
        public bool? Commentable { get; private set; }
        public ForwardType ForwardType { get; private set; }
        public ShareType ShareType { get; private set; }
        public Visibility Visibility { get; private set; }
        public string ViewPassword { get; private set; }
        public bool? ShowOriginalText { get; private set; }

        // 系统标签
        public string SystemTag { get; private set; }

        // 公共标签
        public string PublicTags { get; private set; }

        // 帖子类别
        public string PrivateTag { get; private set; }

        // 圈子里的精华帖子
        public bool CircleGood { get; private set; }

        // 帖子的圈子
        public Guid? CircleId { get; private set; }
        public Circle Circle { get; private set; }

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

        public AppointmentedUserType? AppointmentedUserType { get; private set; }

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
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            UpdatedTime = CreatedTime;
        }

        // constructor to set common properties
        private Post(string text, double? latitude, double? longitude, string locationName, string address, 
            string cityCode, List<PostAttachment> postAttachments, Guid userId)
            : this()
        {
            Text = text;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            Address = address;
            CityCode = cityCode;
            _postAttachments = postAttachments;
            UserId = userId;
        }

        // 构造帖子对象
        private Post(string text, bool? showOriginalText, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            string systemTag, string publicTags, string privateTag, Guid? circleId, double latitude, double longitude, string locationName, string address, string cityCode, 
            List<Guid> friendIds, List<PostAttachment> postAttachments, int score, Guid userId)
            : this(text, latitude, longitude, locationName, address, cityCode, postAttachments, userId)
        {
            // 发送标签更新事件
            AddTagChangedDomainEvent(publicTags);

            Commentable = commentable;
            ForwardType = forwardType;
            ShareType = shareType;
            Visibility = visibility;
            ViewPassword = viewPassword;
            SystemTag = systemTag;
            PublicTags = publicTags;
            PrivateTag = privateTag;
            CircleId = circleId;
            _userPostRelations = friendIds?.Select(id => new UserPostRelation(id, UserPostRelationType.View)).ToList();
            PostType = PostType.Post;
            ShowOriginalText = showOriginalText;
            Score = score;
            LastScoreRefreshedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            // 公开帖子需要审核通过后才能显示
            PostAuthStatus = visibility == Visibility.Public ? PostAuthStatus.Authenticating : PostAuthStatus.Authenticated;
        }

        // 构造约拍对象
        private Post(string text, double? appointedTime, decimal? price, PayerType? payerType, AppointmentedUserType appointmentedUserType, 
            double? latitude, double? longitude, string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId)
            : this(text, latitude, longitude, locationName, address, cityCode, postAttachments, userId)
        {
            AppointedTime = appointedTime;
            Price = price;
            PayerType = payerType;
            AppointmentedUserType = appointmentedUserType;
            PostType = PostType.Appointment;
            PostAuthStatus = PostAuthStatus.Authenticated;
        }

        // 构造约拍交易对象
        private Post(string text, double? appointedTime, decimal? price, PayerType? payerType, AppointmentedUserType appointmentedUserType, 
            double? latitude, double? longitude, string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId, 
            Guid appointmentedUserId, Guid? appointmentedToPostId)
            : this(text, appointedTime, price, payerType, appointmentedUserType, latitude, longitude, locationName, address, cityCode, postAttachments, userId)
        {
            AppointmentedUserId = appointmentedUserId;
            AppointmentedToPostId = appointmentedToPostId;
            PostType = PostType.AppointmentDeal;
            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Created;
            PostAuthStatus = PostAuthStatus.Authenticated;
        }

        // 创建帖子对象
        public static Post CreatePost(string text, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            string systemTag, string publicTags, string privateTag, Guid? circleId, double latitude, double longitude, string locationName, string address, string cityCode,
            List<Guid> friendIds, List<PostAttachment> postAttachments, int score, Guid userId, bool? showOriginalText = null)
        {
            return new Post(text, showOriginalText, commentable, forwardType, shareType, visibility, viewPassword, systemTag, publicTags, privateTag, circleId, latitude, longitude,
                locationName, address, cityCode, friendIds, postAttachments, score, userId);
        }

        // 创建约拍对象
        public static Post CreateAppointment(string text, double? appointedTime, decimal? price, PayerType payerType, AppointmentedUserType appointmentedUserType, 
            double? latitude, double? longitude, string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId)
        {
            return new Post(text, appointedTime, price, payerType, appointmentedUserType, latitude, longitude, locationName, address, cityCode, postAttachments, userId);
        }

        // 创建约拍交易对象
        public static Post CreateAppointmentDeal(string text, double? appointedTime, decimal? price, PayerType? payerType, AppointmentedUserType appointmentedUserType, 
            double? latitude, double? longitude, string locationName, string address, string cityCode, List<PostAttachment> postAttachments, Guid userId, Guid appointmentedUserId, Guid? appointmentedToPostId)
        {
            return new Post(text, appointedTime, price, payerType, appointmentedUserType, latitude, longitude, locationName, address, cityCode, postAttachments, userId, appointmentedUserId, appointmentedToPostId);
        }

        // 更新帖子对象
        public void Update(string text, bool commentable, ForwardType forwardType, ShareType shareType, Visibility visibility, string viewPassword,
            string systemTag, string publicTags, string privateTag, Guid? circleId, double latitude, double longitude, string locationName, string address, string cityCode,
            List<Guid> friendIds, List<PostAttachment> postAttachments, bool? showOriginalText = null)
        {
            // 发送标签更新事件
            AddTagChangedDomainEvent(publicTags);

            Text = text;
            Commentable = commentable;
            ForwardType = forwardType;
            ShareType = shareType;
            Visibility = visibility;
            ViewPassword = viewPassword;
            SystemTag = systemTag;
            PublicTags = publicTags;
            PrivateTag = privateTag;
            CircleId = circleId;
            CircleGood = false; // 帖子编辑后自动取消精华帖
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            Address = address;
            CityCode = cityCode;
            ShowOriginalText = showOriginalText;
            UpdatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            // 公开帖子需要审核通过后才能显示
            PostAuthStatus = visibility == Visibility.Public ? PostAuthStatus.Authenticating : PostAuthStatus.Authenticated;

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
                AddTagChangedDomainEvent(string.Empty);

                _comments.Clear();
                _forwardingPosts.Clear();
            }
            else if (PostType == PostType.Appointment)
            {
                _appointmentedFromPosts.Clear();
            }

            _postAttachments.Clear();
        }

        public void SetForwardPostId(Post forwardedPost)
        {
            if (forwardedPost.PostAuthStatus != PostAuthStatus.Authenticated)
                throw new ClientException("操作失败", new List<string> { $"Post {forwardedPost.Id} is not authenticated, can't forward." });

            ForwardedPostId = forwardedPost.ForwardedPostId ?? forwardedPost.Id;
        }

        public void IncreaseForwardCount(int rewardScore)
        {
            ForwardCount++;
            Score += rewardScore;
        }

        public void CancelAppointmentDeal(Guid userId)
        {
            //  当前操作用户必须为发出该交易的用户
            if (UserId != userId)
                throw new ClientException("操作失败", new List<string> { "Appointment deal is not created by current user." });

            if (AppointmentDealStatus != PostAggregate.AppointmentDealStatus.Created)
                throw new ClientException("操作失败", new List<string> { "Current appointment deal status is not 'Created'." });

            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Canceled;
        }

        public void AcceptAppointmentDeal(Guid userId)
        {
            //  当前操作用户必须为收到该交易的用户
            if (AppointmentedUserId != userId)
                throw new ClientException("操作失败", new List<string> { "Appointmented user is not current user." });

            if (AppointmentDealStatus != PostAggregate.AppointmentDealStatus.Created)
                throw new ClientException("操作失败", new List<string> { "Current appointment deal status is not 'Created'." });

            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Accepted;
        }

        public void RejectAppointmentDeal(Guid userId)
        {
            // 当前操作用户必须为收到该交易的用户
            if (AppointmentedUserId != userId)
                throw new ClientException("操作失败", new List<string> { "Appointmented user is not current user." });

            if (AppointmentDealStatus != PostAggregate.AppointmentDealStatus.Created)
                throw new ClientException("操作失败", new List<string> { "Current appointment deal status is not 'Created'." });

            AppointmentDealStatus = PostAggregate.AppointmentDealStatus.Rejected;
        }

        public void Like(int rewardScore)
        {
            LikeCount++;
            Score += rewardScore;
        }

        public void UnLike(int rewardScore)
        {
            LikeCount = Math.Max(LikeCount - 1, 0);
            Score -= rewardScore;
        }

        public void Share(int rewardScore)
        {
            ShareCount++;
            Score += rewardScore;
        }

        public void IncreaseCommentCount(int rewardScore)
        {
            if (Commentable != null && Commentable.Value)
            {
                CommentCount++;
                Score += rewardScore;
            }
        }

        public void DecreaseCommentCount(int count, int rewardScore)
        {
            CommentCount = Math.Max(CommentCount - count, 0);
            Score -= rewardScore * count;
        }

        public void RemovePrivateTag()
        {
            PrivateTag = null;
        }

        // 设为精华帖
        public void MarkCircleGood()
        {
            CircleGood = true;
        }

        // 取消精华帖
        public void UnMarkCircleGood()
        {
            CircleGood = false;
        }

        public void MoveOutFromCircle()
        {
            CircleGood = false;
            CircleId = null;
        }

        // 浏览帖子加分
        public void View(int rewardScore)
        {
            Score += rewardScore;
        }

        // 审核帖子
        public void Examine(PostAuthStatus status)
        {
            PostAuthStatus = status;
        }

        public void SetCreatedTime(double createdTime)
        {
            CreatedTime = createdTime;
            UpdatedTime = createdTime;
        }

        private void AddDeletedPostDomainEvent()
        {
            var commentIds = _comments.Select(c => c.Id).ToList();
            var deletedPostDomainEvent = new DeletedPostDomainEvent(Id, commentIds);
            AddDomainEvent(deletedPostDomainEvent);
        }

        private void AddTagChangedDomainEvent(string newPublicTags)
        {
            var newTagList = string.IsNullOrWhiteSpace(newPublicTags) ? Array.Empty<string>() : newPublicTags.Split(",");
            var oldTagList = string.IsNullOrWhiteSpace(PublicTags) ? Array.Empty<string>() : PublicTags.Split(",");

            // 本次新增的标签
            var appliedTags = newTagList.Where(t => string.IsNullOrWhiteSpace(PublicTags) || !PublicTags.Contains(t)).ToList();
            // 本次去掉的标签
            var removedTags = oldTagList.Where(t => string.IsNullOrWhiteSpace(newPublicTags) || !newPublicTags.Contains(t)).ToList();

            var tagChangedDomainEvent = new PublicTagChangedDomainEvent(appliedTags, removedTags);
            AddDomainEvent(tagChangedDomainEvent);
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
        You,
        Negotiation
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

    // 帖子审核状态
    public enum PostAuthStatus
    {
        Authenticating,
        Authenticated,
        Rejected
    }

    /// <summary>
    /// 约拍对象类型
    /// </summary>
    public enum AppointmentedUserType
    {
        Photographer,
        Model
    }
}
