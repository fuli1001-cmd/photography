using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    /// <summary>
    /// 约拍值变化事件
    /// </summary>
    public class AppointmentScoreChangedEvent : BaseEvent
    {
        /// <summary>
        /// 约拍值变化的用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 变化值
        /// </summary>
        public int ChangedScore { get; set; }
    }
}
