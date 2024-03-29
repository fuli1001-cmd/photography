﻿using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string Nickname { get; private set; }
        public string Avatar { get; private set; }
        public UserType? UserType { get; private set; }
        public bool IdAuthenticated { get; private set; }

        // 社团认证状态
        public AuthStatus OrgAuthStatus { get; private set; }

        #region BackwardCompatibility: ChatServer needed Property
        public int ChatServerUserId { get; private set; }
        #endregion

        private readonly List<OrderAggregate.Order> _user1Orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> User1Orders => _user1Orders;

        private readonly List<OrderAggregate.Order> _user2Orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> User2Orders => _user2Orders;

        private readonly List<OrderAggregate.Order> _payerOrders = null;
        public IReadOnlyCollection<OrderAggregate.Order> PayerOrders => _payerOrders;

        public User()
        {
            
        }

        public User(Guid id, string nickName, int chatServerUserId)
        {
            Id = id;
            Nickname = nickName;
            ChatServerUserId = chatServerUserId;
        }

        public void Update(string nickName, string avatar, UserType? userType)
        {
            Nickname = nickName;
            Avatar = avatar;
            UserType = userType;
        }

        public void AuthRealName(bool passed)
        {
            IdAuthenticated = passed;
        }

        // 设置用户团体认证状态
        public void SetOrgAuthStatus(AuthStatus status)
        {
            OrgAuthStatus = status;
        }
    }

    public enum UserType
    {
        Photographer,
        Model,
        AmateurModel,
        Other
    }

    public enum AuthStatus
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Rejected
    }
}
