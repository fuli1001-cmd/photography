using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.Seedwork;
using Photography.Services.Post.Infrastructure.EF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class PostRepository : EfRepository<Domain.AggregatesModel.PostAggregate.Post>, IPostRepository
    {
        public PostRepository(PostContext context) : base(context)
        {
            
        }
    }
}
