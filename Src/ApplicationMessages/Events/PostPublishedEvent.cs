using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostPublishedEvent : BaseEvent
    {
        // 发布帖子的用户id
        public Guid UserId { get; set; }

        // 帖子id
        public Guid PostId { get; set; }

        // 帖子中的一张图片
        public string Image { get; set; }
    }
}
