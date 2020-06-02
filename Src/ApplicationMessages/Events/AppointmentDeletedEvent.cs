using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class AppointmentDeletedEvent : BaseEvent
    {
        // 删除约拍的用户id
        public Guid UserId { get; set; }
    }
}
