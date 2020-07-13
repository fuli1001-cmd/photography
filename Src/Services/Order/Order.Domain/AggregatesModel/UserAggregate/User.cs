using Arise.DDD.Domain.SeedWork;
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

        private readonly List<OrderAggregate.Order> _user1Orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> User1Orders => _user1Orders;

        private readonly List<OrderAggregate.Order> _user2Orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> User2Orders => _user2Orders;

        private readonly List<OrderAggregate.Order> _payerOrders = null;
        public IReadOnlyCollection<OrderAggregate.Order> PayerOrders => _payerOrders;

        public User()
        {
            
        }

        public User(string id, string nickName)
        {
            Id = Guid.Parse(id);
            Nickname = nickName;
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
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
