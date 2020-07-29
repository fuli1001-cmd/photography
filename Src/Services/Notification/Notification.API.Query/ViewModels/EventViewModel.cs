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

        public Guid? CircleId { get; set; }

        public string CircleName { get; set; }

        public Guid? OrderId { get; set; }

        /// <summary>
        /// 事件是否已处理
        /// </summary>
        public bool Processed { get; set; }

        /// <summary>
        /// 事件是否已读
        /// </summary>
        public bool Readed { get; set; }
    }

    /// <summary>
    /// 未读事件及其数量
    /// </summary>
    public class UnReadEventCountViewModel
    {
        /// <summary>
        /// 互动事件未读数量
        /// </summary>
        public int Interaction { get; set; }

        /// <summary>
        /// 约拍事件未读数量
        /// </summary>
        public int Appointment { get; set; }

        /// <summary>
        /// 系统事件未读数量
        /// </summary>
        public int System { get; set; }

        /// <summary>
        /// 收到的约拍数量
        /// </summary>
        public int ReceivedAppointmentDeal { get; set; }

        /// <summary>
        /// 发出的约拍数量
        /// </summary>
        public int SentAppointmentDeal { get; set; }
    }
}
