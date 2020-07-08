using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationMessages.Events
{
    public class IdAuthFinished
    {
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
