using Arise.DDD.Infrastructure.Data;
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
            return await _context.UserCommentRelations.SingleOrDefaultAsync(ucr => ucr.UserId == userId && ucr.CommentId == commentId);
        }

        public async Task<List<UserCommentRelation>> GetRelationsByCommentIdsAsync(IEnumerable<Guid> commentIds)
        {
            return await _context.UserCommentRelations.Where(ucr => commentIds.Contains(ucr.CommentId)).ToListAsync();
        }
    }
}
