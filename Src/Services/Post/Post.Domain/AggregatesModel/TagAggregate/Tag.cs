using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.TagAggregate
{
    public class Tag : Entity, IAggregateRoot
    {
        // 标签名
        public string Name { get; private set; }

        // 标签使用数量
        public int Count { get; set; }

        // 私有标签（即帖子类别）所属用户
        public User User { get; private set; }
        public Guid? UserId { get; private set; }

        private readonly List<UserShare> _userShares = null;
        public IReadOnlyCollection<UserShare> UserShares => _userShares;

        public Tag(string name)
        {
            Name = name;
        }

        public Tag(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
        }

        public void IncreaseCount()
        {
            Count++;
        }

        public void DecreaseCount()
        {
            Count = Math.Max(Count - 1, 0);
        }

        public void Delete()
        {
            AddPrivateTagDeletedDomainEvent();
        }

        private void AddPrivateTagDeletedDomainEvent()
        {
            var tagDeletedDomainEvent = new PrivateTagDeletedDomainEvent(UserId.Value, Name);
            AddDomainEvent(tagDeletedDomainEvent);
        }
    }
}
