using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
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

        // 用户圈子多对多关系
        private readonly List<UserCircleRelation> _userCircleRelations = null;
        public IReadOnlyCollection<UserCircleRelation> UserCircleRelations => _userCircleRelations;

        // 圈子的帖子
        private readonly List<PostAggregate.Post> _posts = null;
        public IReadOnlyCollection<PostAggregate.Post> Posts => _posts;
    }
}
