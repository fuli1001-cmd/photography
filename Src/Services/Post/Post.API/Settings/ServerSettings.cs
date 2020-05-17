using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Settings
{
    public class AuthSettings
    {
        // 聊天服务器地址
        public string Authority { get; set; }
        // 文件服务器地址
        public string Audience { get; set; }
    }
}
