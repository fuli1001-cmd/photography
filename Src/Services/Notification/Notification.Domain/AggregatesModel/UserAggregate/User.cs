﻿using Arise.DDD.Domain.SeedWork;
using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Nickname { get; private set; }

        public string Avatar { get; private set; }

        public string RegistrationId { get; private set; }

        // 社团认证状态
        public AuthStatus OrgAuthStatus { get; private set; }

        #region 推送设置
        #region to be deleted
        public PushSetting PushLikeEvent { get; private set; }
        public PushSetting PushReplyEvent { get; private set; }
        public PushSetting PushForwardEvent { get; private set; }
        public PushSetting PushShareEvent { get; private set; }
        public PushSetting PushFollowEvent { get; private set; }
        #endregion

        // 互动通知推送设置
        public PushSetting InteractionEvent { get; private set; }

        // 约拍通知推送设置
        public PushSetting AppointmentEvent { get; private set; }

        // 系统通知推送设置
        public PushSetting SystemEvent { get; private set; }
        #endregion

        private readonly List<Event> _raisedEvents = null;
        public IReadOnlyCollection<Event> RaisedEvents => _raisedEvents;

        private readonly List<Event> _receivedEvents = null;
        public IReadOnlyCollection<Event> ReceivedEvents => _receivedEvents;

        public List<UserRelation> Followers { get; private set; }
        public List<UserRelation> FollowedUsers { get; private set; }

        public User() { }

        public User(Guid id, string nickName)
        {
            Id = id;
            Nickname = nickName;
        }

        public void Update(string nickName, string avatar)
        {
            Nickname = nickName;
            Avatar = avatar;
        }

        public void UpdatePushInfo(string registrationId)
        {
            RegistrationId = registrationId;
        }

        public void ConfigurePushByEventType(EventType eventType, PushSetting setting)
        {
            if (eventType == EventType.Follow)
                PushFollowEvent = setting;
            else if (eventType == EventType.ForwardPost)
                PushForwardEvent = setting;
            else if (eventType == EventType.LikeComment || eventType == EventType.LikePost)
                PushLikeEvent = setting;
            else if (eventType == EventType.ReplyComment || eventType == EventType.ReplyPost)
                PushReplyEvent = setting;
            else if (eventType == EventType.SharePost)
                PushShareEvent = setting;
        }

        public void ConfigurePush(EventCategory eventCategory, PushSetting setting)
        {
            if (eventCategory == EventCategory.Appointment)
                AppointmentEvent = setting;
            else if (eventCategory == EventCategory.Interaction)
                InteractionEvent = setting;
            else if (eventCategory == EventCategory.System)
                SystemEvent = setting;
        }

        // 设置用户团体认证状态
        public void SetOrgAuthStatus(AuthStatus status)
        {
            OrgAuthStatus = status;
        }
    }

    public enum PushSetting
    {
        Open, // 开启
        FollowedOnly, // 只接收我关注的人
        Closed // 关闭
    }

    public enum AuthStatus
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Rejected
    }
}
