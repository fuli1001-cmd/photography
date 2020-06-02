using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class AppointmentPublishedEvent : BaseEvent
    {
        // 发布约拍的用户id
        public Guid UserId { get; set; }
    }
}
