using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostPublishedEvent : BaseEvent
    {
        // 发布帖子的用户id
        public Guid UserId { get; set; }
    }
}
