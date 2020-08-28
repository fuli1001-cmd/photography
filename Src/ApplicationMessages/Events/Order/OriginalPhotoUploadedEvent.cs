using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    /// <summary>
    /// 原片已上传事件
    /// </summary>
    public class OriginalPhotoUploadedEvent : BaseEvent
    {
        /// <summary>
        /// 订单里上传原片的用户id
        /// </summary>
        public Guid UploadPhotoUserId { get; set; }

        /// <summary>
        /// 订单里另一个用户的id
        /// </summary>
        public Guid AnotherUserId { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public Guid OrderId { get; set; }
    }
}
