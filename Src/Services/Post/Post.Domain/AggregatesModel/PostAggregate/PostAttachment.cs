using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class PostAttachment : Entity
    {
        public string Name { get; private set; }
        public string Text { get; private set; }
        public string ContentType { get; private set; }

        public Post Post { get; private set; }
        public Guid PostId { get; private set; }

        public PostAttachment(string name, string text, string contentType)
        {
            Name = name;
            Text = text;
            ContentType = contentType;
        }
    }
}
