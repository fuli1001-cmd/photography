using Photography.Services.User.Domain.BackwardCompatibility.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.BackwardCompatibility
{
    public class CSInstantMsg
    {
        public string fromUserId { get; set; }

        public string toUserId { get; set; }

        public int chatGroupId { get; set; }

        public string msgId { get; set; }

        public int msgType { get; set; }

        public double time { get; set; }

        public string filenames { get; set; }

        public string thumbnails { get; set; }

        public int length { get; set; }

        public string extendData { get; set; }

        public bool isPrivate { get; set; }

        public string content { get; set; }
    }

    public class UserChat
    {
        public Domain.AggregatesModel.UserAggregate.User User { get; set; }

        public PSR_ARS_Chat Chat { get; set; }
    }

    public class ChatMessage
    {
        public IEnumerable<CSInstantMsg> OfflineMsgs { get; set; }

        public IEnumerable<CSInstantMsg> RecentMsgs { get; set; }
    }
}
