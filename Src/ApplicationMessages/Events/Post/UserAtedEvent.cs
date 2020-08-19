using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events.Post
{
    /// <summary>
    /// 用户在帖子中被@的事件
    /// </summary>
    public class UserAtedEvent : BaseEvent
    {
        // 被@的用户id
        public IEnumerable<Guid> AtUserIds { get; set; }

        // 帖子id
        public Guid PostId { get; set; }

        // 发帖用户id
        public Guid PostUserId { get; set; }
    }
}
