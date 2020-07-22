using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Settings
{
    public class AppointmentSettings
    {
        // 发布约拍加分
        public double PublishScore { get; set; }
        
        // 约人及约任务加分
        public double SendDealScore { get; set; }

        // 被约加分
        public double ReceiveDealScore { get; set; }

        // 完成约拍订单加分
        public double FinishDealScore { get; set; }

        // 每日最大发布约拍数
        public int MaxPublishCount { get; set; }

        // 每日最大约人及约任务数
        public int MaxSendDealCount { get; set; }

        // 每日最大收到被约数
        public int MaxReceiveDealCount { get; set; }
    }
}
