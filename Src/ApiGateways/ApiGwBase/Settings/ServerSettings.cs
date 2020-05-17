using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Settings
{
    public class ServerSettings
    {
        /// <summary>
        /// 聊天服务器地址
        /// </summary>
        public string ChatServer { get; set; }

        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public string FileServer { get; set; }

        /// <summary>
        /// 声网appId
        /// </summary>
        public string AgoraAppId { get; set; }

        /// <summary>
        /// 腾讯语音appId
        /// </summary>
        public string TrtcAppId { get; set; }

        /// <summary>
        /// 腾讯语音是否启用
        /// </summary>
        public bool TrtcEnabled { get; set; }
    }
}
