using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    public class OrderFinishedEvent : BaseEvent
    {
        // 订单中确认收货的用户id
        public Guid AcceptUserId { get; set; }

        // 订单中另一个用户id
        public Guid AnotherUserId { get; set; }

        public Guid OrderId { get; set; }
    }
}
