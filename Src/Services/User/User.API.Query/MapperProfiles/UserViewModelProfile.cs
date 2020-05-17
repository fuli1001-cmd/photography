using AutoMapper;
using Photography.Services.User.API.Query.ViewModels;
using Photography.Services.User.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.User.API.Query.MapperProfiles
{
    public class UserViewModelProfile : Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.UserAggregate.User, UserViewModel>();
        }
    }
}
