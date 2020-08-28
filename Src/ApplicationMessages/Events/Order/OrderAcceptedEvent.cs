﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    // 确认约拍
    public class OrderAcceptedEvent : BaseEvent
    {
        // 订单中确认约拍的用户id
        public Guid UserId { get; set; }

        // 该订单的另一个用户id
        public Guid AnotherUserId { get; set; }

        // 约拍任务id
        public Guid DealId { get; set; }

        // 订单id
        public Guid OrderId { get; set; }
    }
}
