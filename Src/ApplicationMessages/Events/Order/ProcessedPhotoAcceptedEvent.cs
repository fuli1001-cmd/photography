using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    /// <summary>
    /// 精修片已被接受事件
    /// </summary>
    public class ProcessedPhotoAcceptedEvent : BaseEvent
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
