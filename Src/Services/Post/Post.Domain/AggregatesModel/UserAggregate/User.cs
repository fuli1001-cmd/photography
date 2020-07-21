using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Nickname { get; private set; }
        public string Avatar { get; private set; }
        public UserType? UserType { get; private set; }

        // 约拍值
        public int Score { get; private set; }

        // 用户所发帖的初始积分
        public int PostScore { get; private set; }

        // 是否已实名认证
        public bool IdAuthenticated { get; private set; }

        public double CreatedTime { get; private set; }

        // 昵称是否已修改过
        public bool NicknameSet { get; private set; }

        // 头像是否已修改过
        public bool AvatarSet { get; private set; }

        // 用户被禁用到的时间点，null表示未禁用
        public DateTime? DisabledTime { get; private set; }

        private readonly List<PostAggregate.Post> _posts = null;
        public IReadOnlyCollection<PostAggregate.Post> Posts => _posts;

        private readonly List<PostAggregate.Post> _appointments = null;
        public IReadOnlyCollection<PostAggregate.Post> Appointments => _appointments;

        private readonly List<Comment> _comments = null;
        public IReadOnlyCollection<Comment> Comments => _comments;

        private readonly List<UserPostRelation> _userPostRelations = null;
        public IReadOnlyCollection<UserPostRelation> UserPostRelations => _userPostRelations;

        private readonly List<UserCommentRelation> _userCommentRelations = null;
        public IReadOnlyCollection<UserCommentRelation> UserCommentRelations => _userCommentRelations;

        // 用户圈子多对多关系
        private readonly List<UserCircleRelation> _userCircleRelations = null;
        public IReadOnlyCollection<UserCircleRelation> UserCircleRelations => _userCircleRelations;

        // 用户作为圈主的圈子
        private readonly List<Circle> _circles = null;
        public IReadOnlyCollection<Circle> Circles => _circles;

        private readonly List<UserShare> _userShares = null;
        public IReadOnlyCollection<UserShare> UserShares => _userShares;

        // Note: self reference many to many relations can't use field, so use property directly here.
        //private readonly List<UserRelation> _followers;
        //public IReadOnlyCollection<UserRelation> Followers => _followers;

        //private readonly List<UserRelation> _followedUsers;
        //public IReadOnlyCollection<UserRelation> FollowedUsers => _followedUsers;

        public List<UserRelation> Followers { get; private set; }
        public List<UserRelation> FollowedUsers { get; private set; }

        public User() { }

        public User(string id, string nickName, int initPostScore)
        {
            Id = Guid.Parse(id);
            Nickname = nickName;
            PostScore = initPostScore; 
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public void Update(string nickName, string avatar, UserType? userType, int rewardPostScore)
        {
            // 更新帖子初始分的逻辑
            // 此块代码只应执行一次，即用户修改了昵称和头像之后，增加他的帖子初始分
            // 因此当NicknameChanged和AvatarChanged都为true时，代表曾经已增加了积分
            // 其中任意一个为false时，则说明还未增加过积分
            if (!NicknameSet || !AvatarSet)
            {
                if (!NicknameSet)
                    NicknameSet = Nickname != nickName;

                if (!AvatarSet)
                    AvatarSet = Avatar != avatar;

                if (NicknameSet && AvatarSet)
                    PostScore += rewardPostScore;
            }

            Nickname = nickName;
            Avatar = avatar;
            UserType = userType;
        }

        public void AuthRealName(bool passed)
        {
            IdAuthenticated = passed;
        }

        public void SetDisabledTime(DateTime? disabledTime)
        {
            DisabledTime = disabledTime;
        }

        /// <summary>
        /// 检查用户是否在被禁期限内
        /// </summary>
        /// <param name="disableHours">禁用小时数</param>
        /// <returns></returns>
        public bool IsDisabled()
        {
            return DisabledTime == null ? false : DateTime.UtcNow <= DisabledTime.Value;
        }
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
