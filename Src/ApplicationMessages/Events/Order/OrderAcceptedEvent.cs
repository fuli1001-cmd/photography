using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    public class OrderAcceptedEvent : BaseEvent
    {
        // 确认订单（即确认约拍任务）的用户id
        public Guid UserId { get; set; }

        // 该订单（约拍任务）的另一个用户id
        public Guid AnotherUserId { get; set; }

        // 约拍任务id
        public Guid DealId { get; set; }
    }
}
