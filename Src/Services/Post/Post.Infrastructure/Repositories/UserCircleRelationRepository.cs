using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserCircleRelationRepository : EfRepository<UserCircleRelation, PostContext>, IUserCircleRelationRepository
    {
        public UserCircleRelationRepository(PostContext context) : base(context)
        {

        }

        public async Task<UserCircleRelation> GetRelationAsync(Guid circleId, Guid userId)
        {
            return await _context.UserCircleRelations.Where(uc => uc.CircleId == circleId && uc.UserId == userId).SingleOrDefaultAsync();
        }

        public async Task<List<UserCircleRelation>> GetRelationsByCircleIdAsync(Guid circleId)
        {
            return await _context.UserCircleRelations.Where(uc => uc.CircleId == circleId).ToListAsync();
        }
    }
}
