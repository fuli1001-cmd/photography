using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleClient.Models
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public int LikeCount { get; set; }
        public int ShareCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Commentable { get; set; }
        public bool Forwardable { get; set; }
        public ShareType ShareType { get; set; }
        public string ViewPassword { get; set; }
        public Location Location { get; set; }
        public List<PostAttachmentViewModel> PostAttachments { get; set; }
        public UserViewModel User { get; set; }
    }

    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public UserType UserType { get; set; }
    }

    public class PostAttachmentViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public PostAttachmentType PostFileType { get; set; }
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public enum ShareType
    {
        All,
        None,
        Friends
    }

    public enum UserType
    {
        Unknown,
        Photographer,
        Model
    }

    public enum PostAttachmentType
    {
        Image,
        Video
    }
}
