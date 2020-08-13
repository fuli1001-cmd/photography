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
        public IdAuthStatus RealNameRegistrationStatus { get; set; }
        public int ChatServerUserId { get; set; }
    }

    public enum IdAuthStatus
    {
        NotAuthenticated = 0,
        Authenticated = 2 // 值设为2以便客户端使用
    }
}
