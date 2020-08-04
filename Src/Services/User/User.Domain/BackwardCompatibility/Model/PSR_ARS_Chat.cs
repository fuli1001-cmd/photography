using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.Domain.BackwardCompatibility.Model
{
    public class PSR_ARS_Chat
    {
        public int IMARSC_Id { get; set; }
        public Nullable<int> IMARSC_FromUserId { get; set; }
        public string IMARSC_FromNickname { get; set; }
        public Nullable<int> IMARSC_ToUserId { get; set; }
        public string IMARSC_ToNickname { get; set; }
        public string IMARSC_Message { get; set; }
        public Nullable<byte> IMARSC_MessageType { get; set; }
        public Nullable<int> IMARSC_CreateYear { get; set; }
        public Nullable<int> IMARSC_CreateMonth { get; set; }
        public Nullable<int> IMARSC_CreateDay { get; set; }
        public string IMARSC_Pictures { get; set; }
        public Nullable<int> IMARSC_GroupId { get; set; }
        public Nullable<System.DateTime> IMARSC_SendTime { get; set; }
        public Nullable<System.DateTime> IMARSC_ReceivedTime { get; set; }
        public Nullable<byte> IMARSC_ReadYorN { get; set; }
        public string IMARSC_MsgId { get; set; }
        public string IMARSC_Thumbnails { get; set; }
        public Nullable<int> IMARSC_FileLength { get; set; }
        public string IMARSC_RefMsgId { get; set; }
        public Nullable<int> IMARSC_RemindUserId { get; set; }
        public Nullable<short> IMARSC_Status { get; set; }
        public string IMARSC_ExtendData { get; set; }
        public Nullable<bool> IMARSC_IsBroadcast { get; set; }
        public bool? IMARSC_IsPrivate { get; set; }
    }
}
