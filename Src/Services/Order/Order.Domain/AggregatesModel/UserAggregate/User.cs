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

        private readonly List<OrderAggregate.Order> _orders = null;
        public IReadOnlyCollection<OrderAggregate.Order> Orders => _orders;
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
