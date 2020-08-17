using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.User
{
    /// <summary>
    /// 用户团体认证状态变化事件
    /// </summary>
    public class UserOrgAuthStatusChangedEvent : BaseEvent
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 改变后的团体认证状态
        /// </summary>
        public int Status { get; set; }
    }
}
