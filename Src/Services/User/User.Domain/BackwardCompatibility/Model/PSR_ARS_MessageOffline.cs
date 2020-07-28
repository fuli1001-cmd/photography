using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.BackwardCompatibility.Model
{
    public class PSR_ARS_MessageOffline
    {
        public int IMARSMNRR_Uid { get; set; }
        public Nullable<int> IMARSMNRR_FromUserId { get; set; }
        public Nullable<int> IMARSMNRR_ToUserId { get; set; }
        public Nullable<int> IMARSMNRR_GroupId { get; set; }
        public Nullable<byte> IMARSMNRR_MsgType { get; set; }
        public string IMARSMNRR_MessageId { get; set; }
        public string IMARSMNRR_Content { get; set; }
        public Nullable<long> IMARSMNRR_Time { get; set; }
        public string IMARSMNRR_Filenames { get; set; }
        public string IMARSMNRR_Thumbnails { get; set; }
        public Nullable<int> IMARSMNRR_Length { get; set; }
        public string IMARSMNRR_ExtendData { get; set; }
        public bool? IMARSMNRR_IsPrivate { get; set; }
        public bool? IMARSMNRR_Encrypted { get; set; }
    }
}
