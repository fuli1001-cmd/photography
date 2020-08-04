using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.API.Query.ViewModels
{
    public class PushSettingsViewModel
    {
        public PushSetting PushLikeEvent { get; set; }
        public PushSetting PushReplyEvent { get; set; }
        public PushSetting PushForwardEvent { get; set; }
        public PushSetting PushShareEvent { get; set; }
        public PushSetting PushFollowEvent { get; set; }

        // 互动通知推送设置
        public PushSetting InteractionEvent { get; set; }

        // 约拍通知推送设置
        public PushSetting AppointmentEvent { get; set; }

        // 系统通知推送设置
        public PushSetting SystemEvent { get; set; }
    }
}
