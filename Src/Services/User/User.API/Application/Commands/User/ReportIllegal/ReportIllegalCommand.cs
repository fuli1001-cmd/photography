using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.User.ReportIllegal
{
    /// <summary>
    /// 举报用户命令
    /// </summary>
    public class ReportIllegalCommand
    {
        /// <summary>
        /// 被举报用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 举报内容
        /// </summary>
        public string Description { get; set; }
    }
}
