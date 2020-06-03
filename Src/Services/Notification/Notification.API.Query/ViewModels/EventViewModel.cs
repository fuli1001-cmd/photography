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

        public string CommentText { get; set; }

        public bool Followed { get; set; }
    }
}
