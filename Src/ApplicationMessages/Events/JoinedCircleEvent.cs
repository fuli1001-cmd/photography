using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class JoinedCircleEvent : BaseEvent
    {
        // 申请加圈的用户id
        public Guid JoinedUserId { get; set; }

        // 圈主id
        public Guid CircleOwnerId { get; set; }

        // 圈子id
        public Guid CircleId { get; set; }

        // 圈子名
        public string CircleName { get; set; }
    }
}
