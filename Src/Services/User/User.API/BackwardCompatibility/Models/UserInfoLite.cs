using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.BackwardCompatibility.Models
{
    public class UserInfoLite
    {
        public int userId { get; set; }
        public string nickname { get; set; }
        public string registrationId { get; set; }
        public string avatar { get; set; }
        public string tel { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string remarks { get; set; }
        public bool muted { get; set; }
        public int stateCode { get; set; }
        public int clientType { get; set; }
        public List<ConnectionInfo> connectionInfos { get; set; }
    }

    public class ConnectionInfo
    {
        public string ServerName { get; set; }
        public string SessionId { get; set; }
        public int ClientType { get; set; }
    }

    public class Token
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string nickname { get; set; }
        public long loginTime { get; set; }
        public int clientType { get; set; }
    }
}
