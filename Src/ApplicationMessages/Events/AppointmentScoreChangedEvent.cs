using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    /// <summary>
    /// 约拍值变化事件
    /// </summary>
    public class AppointmentScoreChangedEvent
    {
        public Guid UserId { get; set; }
    }
}
