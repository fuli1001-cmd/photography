using AutoMapper;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.MapperProfiles
{
    public class AppointmentViewModelProfile : Profile
    {
        public AppointmentViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, AppointmentViewModel>();
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, BasePostViewModel>();
            CreateMap<User, BaseUserViewModel>();
            CreateMap<User, AppointmentUserViewModel>();
            CreateMap<PostAttachment, PostAttachmentViewModel>();
        }
    }
}
