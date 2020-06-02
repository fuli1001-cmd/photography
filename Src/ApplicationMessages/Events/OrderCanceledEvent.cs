using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class OrderCanceledEvent : BaseEvent
    {
        // 操作取消订单的用户id
        public Guid ProcessingUserId { get; set; }

        // 订单中的另外一个用户id
        public Guid AnotherUserId { get; set; }

        // 订单对应的约拍交易id
        public Guid DealId { get; set; }
    }
}
