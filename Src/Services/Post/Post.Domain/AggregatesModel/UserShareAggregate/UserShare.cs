﻿using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate
{
    public class UserShare : Entity, IAggregateRoot
    {
        // 分享者id
        public Guid UserId { get; private set; }
        public User User { get; set; }

        // 被分享的帖子id
        public Guid? PostId { get; private set; }
        public PostAggregate.Post Post { get; private set; }

        // 被分享的帖子分类名称
        public string PrivateTag { get; private set; }

        // 无广告分享
        public bool NoAd { get; private set; }

        // 分享时间
        public double CreatedTime { get; private set; }

        public UserShare()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 分享用户（即分享用户的所有帖子）
        public void ShareUser(Guid userId, bool noAd)
        {
            UserId = userId;
            NoAd = noAd;

            AddUserSharedDomainEvent();
        }

        // 分享指定帖子
        public void SharePost(Guid userId, Guid postId, bool noAd)
        {
            UserId = userId;
            PostId = postId;
            NoAd = noAd;

            AddPostSharedDomainEvent();
        }

        // 分享帖子类别（即分享该类别下的所有帖子，或分享未分类的帖子）
        public void ShareTag(Guid userId, string privateTag, bool noAd)
        {
            UserId = userId;
            PrivateTag = privateTag;
            NoAd = noAd;

            AddTagSharedDomainEvent(privateTag);
        }

        private void AddPostSharedDomainEvent()
        {
            var @event = new PostSharedDomainEvent(Id);
            AddDomainEvent(@event);
        }

        private void AddTagSharedDomainEvent(string privateTag)
        {
            var @event = new PrivateTagSharedDomainEvent(privateTag);
            AddDomainEvent(@event);
        }

        private void AddUserSharedDomainEvent()
        {
            var @event = new UserSharedDomainEvent();
            AddDomainEvent(@event);
        }
    }
}
