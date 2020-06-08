using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.Models
{
    public class PSR_ARS_GroupMembers
    {
        public int IMARSGM_Id { get; set; }
        public Nullable<int> IMARSGM_GroupId { get; set; }
        public Nullable<byte> IMARSGM_GroupStatus { get; set; }
        public Nullable<int> IMARSGM_MemberId { get; set; }
        public Nullable<System.DateTime> IMARSGM_Time { get; set; }
        public string IMARSGM_Nickname { get; set; }
        public Nullable<int> IMARSGM_AddMember { get; set; }
        public Nullable<int> IMARSGM_Speaking { get; set; }
        public Nullable<int> IMARSGM_Mute { get; set; }
    }
}
