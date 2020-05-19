using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Application.Commands.Login
{
    public class ChatServerUser
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string nickname { get; set; }
        public long loginTime { get; set; }
        public int clientType { get; set; }
    }
}
