using AutoMapper;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.API.Query.MapperProfiles
{
    public class CommentViewModelProfile : Profile
    {
        public CommentViewModelProfile()
        {
            CreateMap<Comment, CommentViewModel>();
            CreateMap<User, BaseUserViewModel>();
            CreateMap<User, CommentUserViewModel>();
        }
    }
}
