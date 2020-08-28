using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    // 确认已拍片事件
    public class ConfirmShotEvent : BaseEvent
    {
        // 订单中确认已拍片的用户id
        public Guid ConfirmUserId { get; set; }

        // 该订单的另一个用户id
        public Guid AnotherUserId { get; set; }

        // 订单id
        public Guid OrderId { get; set; }
    }
}
