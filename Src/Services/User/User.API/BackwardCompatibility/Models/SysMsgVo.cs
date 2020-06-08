using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.Models
{
    public class SysMsgVo
    {
        public int type { get; set; }
        public int[] receiverIds { get; set; }
    }
}
