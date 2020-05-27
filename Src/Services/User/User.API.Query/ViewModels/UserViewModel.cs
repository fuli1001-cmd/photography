using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.ViewModels
{
    public class UserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public UserType? UserType { get; set; }
        public string UserName { get; set; }
        public Gender? Gender { get; set; }
        public double? Birthday { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sign { get; set; }
        public int LikedCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
        // 约拍值
        public int Score { get; set; }
        public string Phonenumber { get; set; }

        public int ChatServerUserId { get; set; }
    }

    public class MeViewModel : UserViewModel
    {
        public string Code { get; set; }
        public RealNameRegistrationStatus RealNameRegistrationStatus { get; set; }
    }

    public class BaseUserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
    }

    public enum UserType
    {
        Photographer,
        Model
    }
}
