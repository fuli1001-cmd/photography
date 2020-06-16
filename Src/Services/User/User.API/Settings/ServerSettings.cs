using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Settings
{
    public class ServerSettings
    {
        /// <summary>
        /// 聊天socket服务器地址
        /// </summary>
        public string ChatSocket { get; set; }

        /// <summary>
        /// 聊天socket服务器端口
        /// </summary>
        public int ChatSocketPort { get; set; }

        /// <summary>
        /// 聊天服务API地址
        /// </summary>
        public string ChatAPI { get; set; }

        /// <summary>
        /// 聊天服务API端口
        /// </summary>
        public int ChatAPIPort { get; set; }

        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public string FileServer { get; set; }

        /// <summary>
        /// 文件服务器端口
        /// </summary>
        public int FileServerPort { get; set; }

        /// <summary>
        /// 聊天文件服务器地址
        /// </summary>
        public string ChatFileServer { get; set; }

        /// <summary>
        /// 聊天文件服务器端口
        /// </summary>
        public int ChatFileServerPort { get; set; }

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

        public Version Version { get; set; }
    }

    public class Version
    {
        public UpgradeInfo IOS { get; set; }
        public UpgradeInfo Android { get; set; }
    }

    public class UpgradeInfo
    {
        public string Version { get; set; }

        public string UpgradeUrl { get; set; }

        public int VersionCode { get; set; }

        public int MinVersionCode { get; set; }

        public string UpgradeDesc { get; set; }
    }
}
