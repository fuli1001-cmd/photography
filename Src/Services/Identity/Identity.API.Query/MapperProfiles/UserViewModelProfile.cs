using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.API.Query.MapperProfiles
{
    public class UserViewModelProfile : Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<User, UserViewModel>();
        }
    }
}
