using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Queries.Models
{
    public class UserPost
    {
        public Domain.AggregatesModel.PostAggregate.Post Post { get; set; }
        public User User { get; set; }
    }
}
