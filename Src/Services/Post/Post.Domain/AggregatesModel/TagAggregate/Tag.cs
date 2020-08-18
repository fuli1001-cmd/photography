using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
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

        // 标签类别
        public TagType TagType { get; set; }

        // 公共标签使用数量
        public int Count { get; private set; }

        // 标签顺序（目前仅系统标签用到）
        public int Index { get; set; }

        public double CreatedTime { get; private set; }

        // 私有标签（即帖子类别）所属用户
        public User User { get; private set; }
        public Guid? UserId { get; private set; }

        public Tag()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public Tag(string name) : this()
        {
            Name = name;
            TagType = TagType.Public;
        }

        public Tag(string name, Guid userId) : this()
        {
            Name = name;
            UserId = userId;
            TagType = TagType.Private;
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

    public enum TagType
    {
        System, // 系统类别
        Public, // 公共类别，即帖子标签
        Private // 私有类别，即帖子专辑
    }
}
