using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.AggregatesModel.GroupAggregate
{
    public class Group : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Notice { get; private set; }
        public string Avatar { get; private set; }

        public Guid OwnerId { get; private set; }
        public UserAggregate.User Owner { get; private set; }

        public Group(string name, string avatar, List<Guid> userIds)
        {

        }
    }
}
