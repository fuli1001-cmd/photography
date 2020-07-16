using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class OrderRejectedEvent : BaseEvent
    {
        // 操作拒绝订单的用户id
        public Guid ProcessingUserId { get; set; }

        // 订单中的另外一个用户id
        public Guid AnotherUserId { get; set; }

        // 订单对应的约拍交易id
        public Guid DealId { get; set; }

        // 订单id
        public Guid OrderId { get; set; }

        // 拒绝说明
        public string Description { get; set; }
    }
}
