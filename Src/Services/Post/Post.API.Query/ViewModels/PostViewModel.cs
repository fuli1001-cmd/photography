﻿using AutoMapper;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.ViewModels
{
    public class BasePostViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }

    public class PostViewModel : BasePostViewModel
    {
        public int LikeCount { get; set; }
        public int ShareCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Commentable { get; set; }
        public ForwardType ForwardType { get; set; }
        public ShareType ShareType { get; set; }
        public string ViewPassword { get; set; }
        public Location Location { get; set; }
        public List<PostAttachmentViewModel> PostAttachments { get; set; }
        public UserViewModel User { get; set; }
        public ForwardedPostViewModel ForwardedPost { get; set; }
    }

    public class ForwardedPostViewModel : BasePostViewModel
    {
        public List<PostAttachmentViewModel> PostAttachments { get; set; }
        public BaseUserViewModel User { get; set; }
        public ForwardedPostViewModel ForwardedPost { get; set; }
    }

    public class BaseUserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
    }

    public class UserViewModel : BaseUserViewModel
    {
        public string Avatar { get; set; }
        public UserType UserType { get; set; }
    }

    public class PostAttachmentViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public PostAttachmentType PostAttachmentType { get; set; }
    }

    public class PostViewModelProfile : Profile
    {
        public PostViewModelProfile()
        {
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, PostViewModel>();
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, BasePostViewModel>();
            CreateMap<Domain.AggregatesModel.PostAggregate.Post, ForwardedPostViewModel>();
            CreateMap<User, BaseUserViewModel>();
            CreateMap<User, UserViewModel>();
            CreateMap<PostAttachment, PostAttachmentViewModel>();
        }
    }
}
