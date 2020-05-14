using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public class PostForUser : Entity
    {
        public Guid PostId { get; private set; }
        public Post Post { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public PostForUser(Guid userId)
        {
            UserId = userId;
        }
    }
}
