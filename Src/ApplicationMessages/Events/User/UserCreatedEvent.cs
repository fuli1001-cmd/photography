using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.User
{
    /// <summary>
    /// 用户在用户服务中已创建的事件，此时已有ChatServerUserId
    /// </summary>
    public class UserCreatedEvent : BaseEvent
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public int ChatServerUserId { get; set; }
    }
}
