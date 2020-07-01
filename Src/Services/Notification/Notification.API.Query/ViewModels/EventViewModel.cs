using Photography.Services.Notification.Domain.AggregatesModel.EventAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.API.Query.ViewModels
{
    public class EventViewModel
    {
        // 事件发起人
        public UserViewModel FromUser { get; set; }

        // 事件类型
        public EventType EventType { get; set; }

        // 事件展示图片
        public string Image { get; set; }

        // 事件发生时间
        public double CreatedTime { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        /// <summary>
        /// 评论类容或申请加圈的描述
        /// </summary>
        public string CommentText { get; set; }

        public Guid? CircleId { get; set; }

        public string CircleName { get; set; }

        /// <summary>
        /// 事件是否已处理
        /// </summary>
        public bool Processed { get; set; }
    }
}
