using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate
{
    public class UserShare : Entity, IAggregateRoot
    {
        // 分享者id
        public Guid SharerId { get; private set; }
        public User Sharer { get; private set; }

        // 分享的帖子id
        public Guid? PostId { get; private set; }

        // 分享的帖子类别
        public string PrivateTag { get; private set; }

        public int VisitCount { get; private set; }

        public UserShare(Guid sharerId)
        {
            SharerId = sharerId;
        }

        public UserShare(Guid sharerId, Guid postId) : this(sharerId)
        {
            PostId = postId;
        }

        public UserShare(Guid sharerId, string privateTag) : this(sharerId)
        {
            PrivateTag = privateTag;
        }

        public void IncreaseVisitCount()
        {
            VisitCount++;
        }
    }
}
