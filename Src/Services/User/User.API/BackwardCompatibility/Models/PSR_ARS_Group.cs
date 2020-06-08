using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.Models
{
    public class PSR_ARS_Group
    {
        public int IMARSG_Id { get; set; }
        public string IMARSG_Guid { get; set; }
        public Nullable<byte> IMARSG_Type { get; set; }
        public string IMARSG_Name { get; set; }
        public string IMARSG_Notice { get; set; }
        public Nullable<byte> IMARSG_Status { get; set; }
        public Nullable<int> IMARSG_OwnerId { get; set; }
        public Nullable<int> IMARSG_MembersNum { get; set; }
        public Nullable<System.DateTime> IMARSG_CreateTime { get; set; }
        public string IMARSG_Avatar { get; set; }
        public Nullable<int> IMARSG_AllowAddMemberByAnyone { get; set; }
    }
}
