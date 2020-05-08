using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class Post : Entity, IAggregateRoot
    {
        public string Text { get; private set; }
        public int Likes { get; private set; }
        public int Shares { get; private set; }
        public DateTime Timestamp { get; private set; }

        private readonly List<string> _images;
        public IReadOnlyCollection<string> Images => _images;

        private readonly List<string> _videos;
        public IReadOnlyCollection<string> Videos => _videos;
    }
}
