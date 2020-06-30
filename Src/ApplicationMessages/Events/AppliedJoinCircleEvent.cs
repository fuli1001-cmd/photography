using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class AppliedJoinCircleEvent : BaseEvent
    {
        // 申请加圈的用户id
        public Guid UserId { get; set; }

        public Guid CircleId { get; set; }

        public string CircleName { get; set; }
    }
}
