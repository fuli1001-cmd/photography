using AutoMapper;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class FriendViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
    }
}
