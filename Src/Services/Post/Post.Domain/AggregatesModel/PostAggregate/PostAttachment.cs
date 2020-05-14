using System;
using System.Collections.Generic;
using System.Text;
using Photography.Services.Post.Domain.Seedwork;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class PostAttachment : Entity
    {
        public string Name { get; private set; }
        public string Text { get; private set; }
        public PostAttachmentType PostAttachmentType { get; private set; }

        public Post Post { get; private set; }
        public Guid PostId { get; private set; }

        public PostAttachment(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }

    public enum PostAttachmentType
    {
        Image,
        Video
    }
}
