using Photography.Services.Order.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Order.API.Query.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public UserType? UserType { get; set; }
        public AuthStatus RealNameRegistrationStatus { get; set; }
        public int ChatServerUserId { get; set; }

        // 社团认证状态
        public AuthStatus OrgAuthStatus { get; set; }
    }
}
