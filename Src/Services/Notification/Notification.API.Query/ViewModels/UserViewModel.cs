﻿using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.API.Query.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public bool Followed { get; set; }

        // 社团认证状态
        public AuthStatus OrgAuthStatus { get; set; }
    }
}
