using AutoMapper;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.MapperProfiles
{
    public class PostViewModelProfile : Profile
    {
        public PostViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, PostViewModel>();
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, BasePostViewModel>();
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, ForwardedPostViewModel>();
            CreateMap<User, BaseUserViewModel>();
            CreateMap<User, PostUserViewModel>();
            CreateMap<PostAttachment, PostAttachmentViewModel>();
        }
    }
}
