using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.PostAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserPostRelationRepository : EfRepository<UserPostRelation, PostContext>, IUserPostRelationRepository
    {
        public UserPostRelationRepository(PostContext context) : base(context)
        {

        }

        public async Task<UserPostRelation> GetAsync(Guid userId, Guid postId, UserPostRelationType relationType)
        {
            return await _context.UserPostRelations
                .SingleOrDefaultAsync(upr => upr.UserId == userId && upr.PostId == postId && upr.UserPostRelationType == relationType);
        }

        public async Task<List<UserPostRelation>> GetRelationsByPostIdAsync(Guid postId)
        {
            return await _context.UserPostRelations.Where(upr => upr.PostId == postId).ToListAsync();
        }
    }
}
