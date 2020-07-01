using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Events;
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

        // 入圈时间
        public double JoinTime { get; private set; }
        
        // 置顶显示该圈
        public bool Topping { get; private set; }

        public UserCircleRelation()
        {
            JoinTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            Topping = false;
        }

        public UserCircleRelation(Guid userId) : this()
        {
            UserId = userId;
        }

        public UserCircleRelation(Guid userId, Guid circleId) : this()
        {
            UserId = userId;
            CircleId = circleId;
        }

        public void ToppingCircle()
        {
            Topping = true;
        }

        public void UnToppingCircle()
        {
            Topping = false;
        }

        public void Join()
        {
            AddJoinedCircleDomainEvent();
        }

        public void Quit()
        {
            AddQuittedCircleDomainEvent();
        }

        private void AddJoinedCircleDomainEvent()
        {
            var @event = new JoinedCircleDomainEvent(CircleId);
            AddDomainEvent(@event);
        }

        private void AddQuittedCircleDomainEvent()
        {
            var @event = new QuittedCircleDomainEvent(CircleId);
            AddDomainEvent(@event);
        }
    }
}
