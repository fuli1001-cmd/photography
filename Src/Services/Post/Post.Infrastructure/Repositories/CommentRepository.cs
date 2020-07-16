using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.CommentAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class CommentRepository : EfRepository<Comment, PostContext>, ICommentRepository
    {
        public CommentRepository(PostContext context) : base(context)
        {

        }

        public async Task<Comment> GetCommentAsync(Guid commentId)
        {
            return await _context.Comments
                .Where(c => c.Id == commentId)
                .Include(c => c.SubComments)
                .Include(c => c.Post)
                .FirstOrDefaultAsync();
        }
    }
}
