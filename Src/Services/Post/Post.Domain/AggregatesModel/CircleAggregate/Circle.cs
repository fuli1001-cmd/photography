using Arise.DDD.Domain.Exceptions;
using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.CircleAggregate
{
    public class Circle : Entity, IAggregateRoot
    {
        // 圈子名
        public string Name { get; private set; }

        // 圈子简介
        public string Description { get; private set; }

        // 入圈审核
        public bool VerifyJoin { get; private set; }

        // 圈子封面图
        public string BackgroundImage { get; private set; }

        // 圈子人数
        public int UserCount { get; private set; }

        public Guid OwnerId { get; private set; }
        public User Owner { get; private set; }

        // 用户圈子多对多关系
        private readonly List<UserCircleRelation> _userCircleRelations = null;
        public IReadOnlyCollection<UserCircleRelation> UserCircleRelations => _userCircleRelations;

        // 圈子的帖子
        private readonly List<PostAggregate.Post> _posts = null;
        public IReadOnlyCollection<PostAggregate.Post> Posts => _posts;

        public Circle(string name, string description, bool verifyJoin, string backgroundImage, Guid ownerId)
        {
            Name = name;
            Description = description;
            VerifyJoin = verifyJoin;
            BackgroundImage = backgroundImage;
            OwnerId = ownerId;
            _userCircleRelations = new List<UserCircleRelation>();
            _userCircleRelations.Add(new UserCircleRelation(OwnerId));

            IncreaseUserCount();
        }

        public void Update(string name, string description, bool verifyJoin, string backgroundImage, Guid ownerId)
        {
            if (ownerId != OwnerId)
                throw new ClientException("操作失败", new List<string> { $"Circle {Id} does not belong to user {ownerId}" });

            Name = name;
            Description = description;
            VerifyJoin = verifyJoin;
            BackgroundImage = backgroundImage;
        }

        public void Delete(Guid ownerId)
        {
            if (ownerId != OwnerId)
                throw new ClientException("操作失败", new List<string> { $"Circle {Id} does not belong to user {ownerId}" });

            _posts.ForEach(p => p.MoveOutFromCircle());

            AddCircleDeletedDomainEvent();
        }

        public void IncreaseUserCount()
        {
            UserCount++;
        }

        public void DecreaseUserCount()
        {
            UserCount = Math.Max(0, UserCount - 1);
        }

        private void AddCircleDeletedDomainEvent()
        {
            var @event = new CircleDeletedDomainEvent(Id);
            AddDomainEvent(@event);
        }
    }
}
