using AutoMapper;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class BaseUserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public IdAuthStatus RealNameRegistrationStatus { get; set; }
        public AuthStatus OrgAuthStatus { get; set; }
    }

    public class PostUserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public UserType? UserType { get; set; }
        public bool Followed { get; set; }
    }

    public class AppointmentUserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public UserType? UserType { get; set; }
        public int Score { get; set; }
    }

    public class CommentUserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
    }

    public enum IdAuthStatus
    {
        NotAuthenticated = 0,
        Authenticated = 2 // 值设为2以便客户端使用
    }
}
