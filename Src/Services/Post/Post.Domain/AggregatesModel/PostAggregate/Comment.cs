using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class Comment : Entity
    {
        public string Text { get; private set; }
        public int Likes { get; private set; }
        public DateTime Timestamp { get; private set; }
    }
}
