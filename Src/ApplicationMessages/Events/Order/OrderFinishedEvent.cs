using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Order
{
    public class OrderFinishedEvent : BaseEvent
    {
        // 订单中的用户1id
        public Guid User1Id { get; set; }

        // 订单中的用户2id
        public Guid User2Id { get; set; }
    }
}
