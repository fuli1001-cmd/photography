using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.EventAggregate
{
    public class Event : Entity, IAggregateRoot
    {
        // 事件发起人id
        public Guid FromUserId { get; private set; }

        // 事件发起人昵称
        public string FromUserNickName { get; private set; }

        // 事件接收人id
        public Guid ToUserId { get; private set; }

        // 事件类型
        public EventType EventType { get; private set; }

        // 事件展示图片
        public string Image { get; private set; }

        // 事件发生时间
        public double CreatedTime { get; private set; }
    }

    public enum EventType
    {
        ReplyPost, // 回复帖子
        LikePost, // 点赞帖子
        ForwardPost, // 转发帖子
        SharePost, // 分享帖子
        ReplyComment, // 回复评论
        Follow // 关注
    }
}
