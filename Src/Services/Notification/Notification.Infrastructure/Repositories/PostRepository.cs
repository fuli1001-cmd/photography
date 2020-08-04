using Arise.DDD.Infrastructure.Data;
using Photography.Services.Notification.Domain.AggregatesModel.PostAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class PostRepository : EfRepository<Post, NotificationContext>, IPostRepository
    {
        public PostRepository(NotificationContext context) : base(context)
        {

        }
    }
}
