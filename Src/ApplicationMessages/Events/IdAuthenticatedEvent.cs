using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class IdAuthenticatedEvent : BaseEvent
    {
        /// <summary>
        /// 操作员id
        /// </summary>
        public Guid OperatorId { get; set; }

        /// <summary>
        /// 认证用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 认证结果，true：通过，false：失败
        /// </summary>
        public bool Passed { get; set; }
    }
}
