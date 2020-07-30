using Arise.DDD.Domain.SeedWork;
using Photography.Services.Notification.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Domain.AggregatesModel.EventAggregate
{
    public class Event : Entity, IAggregateRoot
    {
        // 事件发起人
        public Guid FromUserId { get; private set; }
        public User FromUser { get; private set; }

        // 事件接收人
        public Guid ToUserId { get; private set; }
        public User ToUser { get; private set; }

        // 事件类型
        public EventType EventType { get; private set; }

        // 事件发生时间
        public double CreatedTime { get; private set; }

        // 事件关联帖子
        public Guid? PostId { get; private set; }
        public Post Post { get; private set; }

        // 事件关联评论id
        public Guid? CommentId { get; private set; }

        // 评论类容或申请加圈的描述
        public string CommentText { get; private set; }

        public Guid? CircleId { get; private set; }

        public string CircleName { get; private set; }

        public Guid? OrderId { get; private set; }

        public bool Processed { get; private set; }

        public bool Readed { get; private set; }

        public Event()
        {
            CreatedTime = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public Event(Guid fromUserId, Guid toUserId, EventType eventType, Guid? postId, Guid? commentId, string commentText, Guid? circleId, string circleName, Guid? orderId)
            : this()
        {
            FromUserId = fromUserId;
            ToUserId = toUserId;
            EventType = eventType;
            PostId = postId;
            CommentId = commentId;
            CommentText = commentText;
            CircleId = circleId;
            CircleName = circleName;
            OrderId = orderId;
        }

        public void MarkAsProcessed()
        {
            Processed = true;
        }

        public void MarkAsReaded()
        {
            Readed = true;
        }
    }

    public enum EventType
    {
        ReplyPost, // 回复帖子
        LikePost, // 点赞帖子
        ForwardPost, // 转发帖子
        SharePost, // 分享帖子
        ReplyComment, // 回复评论
        LikeComment, // 点赞评论
        Follow, // 关注
        ApplyJoinCircle, // 申请加圈
        JoinCircle, // 加入圈子
        DeletePost, // 删除帖子
        CancelOrder, // 取消订单
        RejectOrder, // 拒绝订单
        IdAuthenticated, // 实名认证通过
        IdRejected, // 实名认证被拒
        CircleOwnerChanged // 圈主改变
    }

    // 事件类别
    public enum EventCategory
    {
        Interaction, // 互动
        Appointment, // 约拍
        System // 系统
    }
}
