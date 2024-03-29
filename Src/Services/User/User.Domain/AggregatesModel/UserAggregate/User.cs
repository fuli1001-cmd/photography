﻿using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.User.Domain.AggregatesModel.FeedbackAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string UserName { get; private set; }

        public string Nickname { get; private set; }

        public string Phonenumber { get; private set; }

        public string Avatar { get; private set; }

        public string BackgroundImage { get; private set; }

        public Gender? Gender { get; private set; }

        public double? Birthday { get; private set; }

        public UserType? UserType { get; private set; }

        public string Province { get; private set; }

        public string City { get; private set; }

        public string Sign { get; private set; }

        // 关注的用户数量
        public int FollowingCount { get; private set; }

        // 关注者数量
        public int FollowerCount { get; private set; }

        // 发布的帖子数量
        public int PostCount { get; private set; }

        // 发布的约拍数量
        public int AppointmentCount { get; private set; }

        // 被赞数量
        public int LikedCount { get; private set; }

        // 赞过的帖子数量
        public int LikedPostCount { get; private set; }

        // 进行中的订单数量（除去待确认、已完成、已拒绝和已取消外的订单数量）
        public int OngoingOrderCount { get; private set; }

        public int WaitingForConfirmOrderCount { get; private set; }

        // 约拍值
        public int Score { get; private set; }

        // 邀请码
        public string Code { get; private set; }

        // 实名认证状态
        public IdAuthStatus RealNameRegistrationStatus { get; private set; }

        public string IdCardFront { get; private set; }

        public string IdCardBack { get; private set; }

        public string IdCardHold { get; private set; }

        public bool ViewFollowersAllowed { get; private set; }

        public bool ViewFollowedUsersAllowed { get; private set; }

        public double CreatedTime { get; private set; }

        public double UpdatedTime { get; private set; }

        // 用户被禁用到的时间点，null表示未禁用
        public DateTime? DisabledTime { get; private set; }

        // 被禁次数
        public int DisabledCount { get; private set; }

        #region 团体认证
        // 社团类型
        public int? OrgType { get; private set; }

        // 社团所在高校名称
        public string OrgSchoolName { get; private set; }

        // 社团名称
        public string OrgName { get; private set; }

        // 认证信息（社团介绍）
        public string OrgDesc { get; private set; }

        // 社团运营者姓名
        public string OrgOperatorName { get; private set; }

        // 社团运营者手机号
        public string OrgOperatorPhoneNumber { get; private set; }

        // 社团照片
        public string OrgImage { get; private set; }

        // 社团认证状态
        public IdAuthStatus OrgAuthStatus { get; private set; }

        // 社团认证失败原因
        public string OrgAuthMessage { get; private set; }
        #endregion

        #region BackwardCompatibility: ChatServer needed Property
        public int ChatServerUserId { get; private set; }
        public string RegistrationId { get; private set; }
        public int ClientType { get; set; }
        #endregion

        //private readonly List<UserRelation> _fromUserRelations = null;
        //public IReadOnlyCollection<UserRelation> FromUserRelations => _fromUserRelations;

        //private readonly List<UserRelation> _toUserRelations = null;
        //public IReadOnlyCollection<UserRelation> ToUserRelations => _toUserRelations;

        // 用户作为群主的群
        private readonly List<Group> _groups = null;
        public IReadOnlyCollection<Group> Groups => _groups;

        // 与group的多对多关系
        private readonly List<GroupUser> _groupUsers = null;
        public IReadOnlyCollection<GroupUser> GroupUsers => _groupUsers;

        // 用户的反馈信息
        private readonly List<Feedback> _feedbacks = null;
        public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks;

        public User()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            UpdatedTime = CreatedTime;
            ViewFollowedUsersAllowed = true;
            ViewFollowersAllowed = true;
            OrgAuthStatus = IdAuthStatus.NoIdCard;
        }

        public User(string id, string userName, string phonenumber, string code, string nickName) : this()
        {
            Id = Guid.Parse(id);
            UserName = userName;
            Phonenumber = phonenumber;
            Code = code;
            Nickname = nickName;
        }

        public void Update(string nickname, Gender? gender, double? birthday, UserType? userType, 
            string province, string city, string sign, string avatar)
        {
            if (!string.IsNullOrEmpty(nickname) && nickname.Contains(" "))
                throw new ClientException("昵称不能包含空格");

            Nickname = nickname;
            Gender = gender;
            Birthday = birthday;
            UserType = userType;
            Province = province;
            City = city;
            Sign = sign;
            Avatar = avatar;
        }

        public void UpdateBackground(string background)
        {
            BackgroundImage = background;
        }

        public void IncreasePostCount()
        {
            PostCount++;
        }

        public void DecreasePostCount()
        {
            PostCount = Math.Max(0, PostCount - 1);
        }

        //public void IncreaseAppointmentCount()
        //{
        //    AppointmentCount++;
        //}

        //public void DecreaseAppointmentCount()
        //{
        //    AppointmentCount = Math.Max(0, AppointmentCount - 1);
        //}

        public void IncreaseFollowerCount()
        {
            FollowerCount++;
        }

        public void DecreaseFollowerCount()
        {
            FollowerCount = Math.Max(0, FollowerCount - 1);
        }

        public void IncreaseFollowingCount()
        {
            FollowingCount++;
        }

        public void DecreaseFollowingCount()
        {
            FollowingCount = Math.Max(0, FollowingCount - 1);
        }

        public void IncreaseLikedCount()
        {
            LikedCount++;
        }

        public void DecreaseLikedCount()
        {
            LikedCount = Math.Max(0, LikedCount - 1);
        }

        //public void IncreaseLikedPostCount()
        //{
        //    LikedPostCount++;
        //}

        //public void DecreaseLikedPostCount()
        //{
        //    LikedPostCount = Math.Max(0, LikedPostCount - 1);
        //}

        public void IncreaseOngoingOrderCount()
        {
            OngoingOrderCount++;
        }

        public void DecreaseOngoingOrderCount()
        {
            OngoingOrderCount = Math.Max(0, OngoingOrderCount - 1);
        }

        public void IncreaseWaitingForConfirmOrderCount()
        {
            WaitingForConfirmOrderCount++;
        }

        public void DecreaseWaitingForConfirmOrderCount()
        {
            WaitingForConfirmOrderCount = Math.Max(0, WaitingForConfirmOrderCount - 1);
        }

        //public void IncreaseShootingStageOrderCount()
        //{
        //    ShootingStageOrderCount++;
        //}

        //public void DecreaseShootingStageOrderCount()
        //{
        //    ShootingStageOrderCount = Math.Max(0, ShootingStageOrderCount - 1);
        //}

        //public void IncreaseSelectionStageOrderCount()
        //{
        //    SelectionStageOrderCount++;
        //}

        //public void DecreaseSelectionStageOrderCount()
        //{
        //    SelectionStageOrderCount = Math.Max(0, SelectionStageOrderCount - 1);
        //}

        //public void IncreaseProductionStageOrderCount()
        //{
        //    ProductionStageOrderCount++;
        //}

        //public void DecreaseProductionStageOrderCount()
        //{
        //    ProductionStageOrderCount = Math.Max(0, ProductionStageOrderCount - 1);
        //}

        public void SetChatServerProperties(int clientType, string registrationId)
        {
            ClientType = clientType;
            RegistrationId = registrationId;
        }

        public void AllowViewFollowers(bool value)
        {
            ViewFollowersAllowed = value;
        }

        public void AllowViewFollowedUsers(bool value)
        {
            ViewFollowedUsersAllowed = value;
        }

        public void SetIdCard(string idCardFront, string idCardBack, string idCardHold)
        {
            if (RealNameRegistrationStatus == IdAuthStatus.Authenticating)
                throw new ClientException("操作失败", new List<string> { $"Authentication in process." });

            if (RealNameRegistrationStatus == IdAuthStatus.Authenticated)
                throw new ClientException("操作失败", new List<string> { $"Already Authenticated." });

            IdCardFront = idCardFront;
            IdCardBack = idCardBack;
            IdCardHold = idCardHold;

            RealNameRegistrationStatus = IdAuthStatus.Authenticating;
        }

        public void AuthRealName(bool passed)
        {
            RealNameRegistrationStatus = passed ? IdAuthStatus.Authenticated : IdAuthStatus.Rejected;
        }

        public void Disable(double disableHours)
        {
            // 如果用户目前处于禁用状态，则直接返回，不改变被禁到的时间
            if (DisabledTime != null && DisabledTime.Value > DateTime.UtcNow)
                return;

            // 按指数方式设置被禁到的时间，并增加被禁次数用于下次计算被禁到的时间
            DisabledTime = DateTime.UtcNow.AddHours(disableHours * Math.Pow(2, DisabledCount));
            DisabledCount++;
        }

        public void Enable()
        {
            DisabledTime = null;
        }

        public void AddAppointmentScore(int score)
        {
            Score += score;
        }

        // 设置团体认证信息
        public void SetOrgAuthInfo(int orgType, string orgSchoolName, string orgName, string orgDesc, string orgOperatorName, string orgOperatorPhoneNumber, string orgImage)
        {
            OrgType = orgType;
            OrgSchoolName = orgSchoolName;
            OrgName = orgName;
            OrgDesc = orgDesc;
            OrgOperatorName = orgOperatorName;
            OrgOperatorPhoneNumber = orgOperatorPhoneNumber;
            OrgImage = orgImage;
            SetOrgAuthStatus(IdAuthStatus.Authenticating, null);
        }

        // 设置用户团体认证状态
        public void SetOrgAuthStatus(IdAuthStatus status, string message)
        {
            OrgAuthStatus = status;

            if (status == IdAuthStatus.Rejected)
                OrgAuthMessage = message;
            else
                OrgAuthMessage = null;
        }
    }

    public enum UserType
    {
        Photographer,
        Model,
        AmateurModel,
        Other
    }

    public enum Gender
    {
        Female,
        Male
    }

    public enum IdAuthStatus
    {
        NoIdCard,
        Authenticating,
        Authenticated,
        Rejected
    }
}
