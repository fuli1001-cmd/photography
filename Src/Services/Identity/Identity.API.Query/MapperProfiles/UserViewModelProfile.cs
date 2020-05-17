using AutoMapper;
using Photography.Services.Identity.API.Query.ViewModels;
using Photography.Services.Identity.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Identity.API.Query.MapperProfiles
{
    public class UserViewModelProfile : Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
