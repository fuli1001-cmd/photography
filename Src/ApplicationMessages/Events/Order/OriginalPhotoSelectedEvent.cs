using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    /// <summary>
    /// 原片已选择事件
    /// </summary>
    public class OriginalPhotoSelectedEvent : BaseEvent
    {
        /// <summary>
        /// 订单中选片的用户id
        /// </summary>
        public Guid SelectPhotoUserId { get; set; }

        /// <summary>
        /// 订单中另一个用户id
        /// </summary>
        public Guid AnotherUserId { get; set; }

        public Guid OrderId { get; set; }
    }
}
