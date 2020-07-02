using Arise.DDD.Domain.SeedWork;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
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

        // 被分享的帖子分类id
        public Guid? TagId { get; private set; }
        public Tag Tag { get; private set; }

        // 分享帖子、帖子分类、用户时，该值为false，分享未分类帖子时，改值为true
        public bool UnSpecifiedTag { get; private set; }

        // 分享时间
        public double CreatedTime { get; private set; }

        public UserShare()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        // 分享用户（即分享用户的所有帖子）
        public void ShareUser(Guid userId)
        {
            UserId = userId;
        }

        // 分享指定帖子
        public void SharePost(Guid userId, Guid postId)
        {
            UserId = userId;
            PostId = postId;
        }

        // 分享帖子类别（即分享该类别下的所有帖子）
        public void ShareTag(Guid userId, Guid tagId)
        {
            UserId = userId;
            TagId = tagId;
        }

        // 分享未分类帖子
        public void ShareUnSpecifiedTag(Guid userId)
        {
            UserId = userId;
            UnSpecifiedTag = true;
        }
    }
}
