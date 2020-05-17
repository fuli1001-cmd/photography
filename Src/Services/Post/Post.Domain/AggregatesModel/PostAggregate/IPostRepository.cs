using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.PostAggregate
{
    public interface IPostRepository : IRepository<Post>
    {
        void LoadUser(Post post);
    }
}
