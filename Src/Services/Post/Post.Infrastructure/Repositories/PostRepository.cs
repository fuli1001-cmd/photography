using Arise.DDD.Infrastructure;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class PostRepository : EfRepository<Domain.AggregatesModel.PostAggregate.Post, PostContext>, IPostRepository
    {
        public PostRepository(PostContext context) : base(context)
        {
            
        }

        public void LoadUser(Domain.AggregatesModel.PostAggregate.Post post)
        {
            _context.Entry(post).Reference(p => p.User).Load();
        }
    }
}
