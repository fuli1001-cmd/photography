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
        /// 订单用户1的id
        /// </summary>
        public Guid User1Id { get; set; }

        /// <summary>
        /// 订单用户2的id
        /// </summary>
        public Guid User2Id { get; set; }
    }
}
