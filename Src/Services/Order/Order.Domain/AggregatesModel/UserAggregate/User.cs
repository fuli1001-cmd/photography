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
        public UserType UserType { get; private set; }

        private readonly List<OrderAggregate.Order> _user1Orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> User1Orders => _user1Orders;

        private readonly List<OrderAggregate.Order> _user2Orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> User2Orders => _user2Orders;

        private readonly List<OrderAggregate.Order> _payerOrders = null;
        public IReadOnlyCollection<OrderAggregate.Order> PayerOrders => _payerOrders;
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
