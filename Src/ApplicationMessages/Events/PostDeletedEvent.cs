﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class PostDeletedEvent : BaseEvent
    {
        /// <summary>
        /// 被删除帖子所属用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 操作删除的用户id
        /// </summary>
        public Guid OperatorId { get; set; }
    }
}
