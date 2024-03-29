﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Identity.API.Query.ViewModels
{
    public class UserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public UserType? UserType { get; set; }
        public string UserName { get; set; }
        public string Phonenumber { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sign { get; set; }
        public int LikedCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
        // 约拍值
        public int Score { get; set; }
        public string Code { get; set; }
        public bool RealNameRegistered { get; set; }
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
