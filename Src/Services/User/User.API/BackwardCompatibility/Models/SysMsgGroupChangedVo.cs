using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.Models
{
    public class SysMsgGroupChangedVo : SysMsgVo
    {
        public int groupId { get; set; }
        public string operatorName { get; set; }
        public int operatorId { get; set; }
        public int[] changedMemberIds { get; set; }
        public string[] changedMemberNames { get; set; }
    }
}
