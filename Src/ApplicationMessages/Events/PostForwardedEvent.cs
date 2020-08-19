using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostForwardedEvent : BaseEvent
    {
        public List<ForwardInfo> ForwardInfos { get; set; }
    }

    public class ForwardInfo
    {
        // 转发帖子的用户id
        public Guid ForwardUserId { get; set; }

        // 被转发的帖子用户id
        public Guid OriginalPostUserId { get; set; }

        // 被转发的帖子id
        public Guid OriginalPostId { get; set; }

        // 转发产生的新帖子的id
        public Guid NewPostId { get; set; }

        // 被@的用户id
        public IEnumerable<Guid> AtUserIds { get; set; }
    }
}
