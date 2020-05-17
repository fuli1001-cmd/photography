using AutoMapper;
using Photography.Services.User.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.MapperProfiles
{
    public class FriendViewModelProfile : Profile
    {
        public FriendViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.UserAggregate.User, FriendViewModel>();
        }
    }
}
