using AutoMapper;
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
        public double Timestamp { get; set; }
        public bool Commentable { get; set; }
        public ForwardType ForwardType { get; set; }
        public ShareType ShareType { get; set; }
        public string ViewPassword { get; set; }
        public bool Liked { get; set; }
        public bool? ShowOriginalText { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<PostAttachmentViewModel> PostAttachments { get; set; }
        public PostUserViewModel User { get; set; }
        public ForwardedPostViewModel ForwardedPost { get; set; }
    }

    public class ForwardedPostViewModel : BasePostViewModel
    {
        public List<PostAttachmentViewModel> PostAttachments { get; set; }
        public BaseUserViewModel User { get; set; }
        public ForwardedPostViewModel ForwardedPost { get; set; }
    }

    public class PostAttachmentViewModel
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public AttachmentType AttachmentType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Thumbnail { get; set; }
    }
}
