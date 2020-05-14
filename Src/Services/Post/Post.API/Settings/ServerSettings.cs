using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Settings
{
    public class ServerSettings
    {
        // 聊天服务器地址
        public string ChatServer { get; set; }
        // 文件服务器地址
        public string FileServer { get; set; }
        // 声网appId
        public string AgoraAppId { get; set; }
        // 腾讯语音appId
        public string TrtcAppId { get; set; }
        public bool TrtcEnabled { get; set; }
    }
}
