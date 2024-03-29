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
        public IEnumerable<PostAttachmentViewModel> PostAttachments { get; set; }
    }

    public class PostViewModel : BasePostViewModel
    {
        public int LikeCount { get; set; }
        public int ShareCount { get; set; }
        public int CommentCount { get; set; }
        public double CreatedTime { get; set; }
        public double UpdatedTime { get; set; }
        public bool Commentable { get; set; }
        public ForwardType ForwardType { get; set; }
        public ShareType ShareType { get; set; }
        public string ViewPassword { get; set; }
        public bool Liked { get; set; }
        public bool? ShowOriginalText { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public Visibility Visibility { get; set; }
        public string CityCode { get; set; }
        public IEnumerable<Guid> FriendIds { get; set; }
        // 系统标签
        public string SystemTag { get; set; }
        // 公共标签
        public string PublicTags { get; set; }
        // 私有标签
        public string PrivateTag { get; set; }
        // 圈子
        public PostCircleViewModel Circle { get; set; }

        // 帖子审核状态
        public PostAuthStatus PostAuthStatus { get; set; }

        /// <summary>
        /// 是否是圈子里的精华帖
        /// </summary>
        public bool CircleGood { get; set; }

        public PostUserViewModel User { get; set; }
        public ForwardedPostViewModel ForwardedPost { get; set; }
    }

    public class ForwardedPostViewModel : BasePostViewModel
    {
        public BaseUserViewModel User { get; set; }
        public ForwardedPostViewModel ForwardedPost { get; set; }
        public string PublicTags { get; set; }
    }

    public class PostAttachmentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public AttachmentType AttachmentType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Thumbnail { get; set; }

        /// <summary>
        /// 仅仅自己可见
        /// </summary>
        public bool IsPrivate { get; set; }
    }
}
