using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    /// <summary>
    /// 用户被禁事件
    /// </summary>
    public class UserDisabledEvent : BaseEvent
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 被禁用到的时间点
        /// </summary>
        public DateTime? DisabledTime { get; set; }
    }
}
