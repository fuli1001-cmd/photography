using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.UserCommentRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserCommentRelationRepository : EfRepository<UserCommentRelation, PostContext>, IUserCommentRelationRepository
    {
        public UserCommentRelationRepository(PostContext context) : base(context)
        {

        }

        public async Task<UserCommentRelation> GetAsync(Guid userId, Guid commentId)
        {
            var ucrs = await _context.UserCommentRelations.Where(ucr => ucr.UserId == userId && ucr.CommentId == commentId).ToListAsync();
            if (ucrs.Count > 0)
                return ucrs[0];
            else
                return null;
        }
    }
}
