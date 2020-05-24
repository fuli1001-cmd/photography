using Arise.DDD.Infrastructure;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class CommentRepository : EfRepository<Comment, PostContext>, ICommentRepository
    {
        public CommentRepository(PostContext context) : base(context)
        {

        }
    }
}
