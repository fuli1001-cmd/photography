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
    }
}
