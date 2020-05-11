using System;
using System.Collections.Generic;
using System.Text;
using Photography.Services.Post.Domain.Seedwork;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class PostAttachment : Entity
    {
        public string Url { get; private set; }
        public string Text { get; private set; }
        public PostAttachmentType PostAttachmentType { get; private set; }

        public Post Post { get; private set; }
        public Guid PostId { get; private set; }
    }

    public enum PostAttachmentType
    {
        Image,
        Video
    }
}
