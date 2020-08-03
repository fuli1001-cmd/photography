using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.User
{
    /// <summary>
    /// 用户推送信息改变事件
    /// </summary>
    public class UserPushInfoChangedEvent : BaseEvent
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 新的推送id
        /// </summary>
        public string RegistrationId { get; set; }
    }
}
