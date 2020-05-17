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

        public async Task<UserPostRelation> GetAsync(Guid userId, Guid postId)
        {
            var uprs = await _context.UserPostRelations.Where(upr => upr.UserId == userId && upr.PostId == postId).ToListAsync();
            if (uprs.Count > 0)
                return uprs[0];
            else
                return null;
        }
    }
}
