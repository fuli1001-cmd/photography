using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate
{
    /// <summary>
    /// 用户与圈子的多对多关系
    /// </summary>
    public class UserCircleRelation : Entity, IAggregateRoot
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public Guid CircleId { get; private set; }
        public Circle Circle { get; private set; }
    }
}
