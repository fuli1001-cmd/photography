using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class UserRelationRepository : EfRepository<UserRelation, UserContext>, IUserRelationRepository
    {
        public UserRelationRepository(UserContext context) : base(context)
        {

        }

        public async Task<UserRelation> GetAsync(Guid followerId, Guid followedUserId)
        {
            var urs = await _context.UserRelations.Where(ur => ur.FollowerId == followerId && ur.FollowedUserId == followedUserId).ToListAsync();
            if (urs.Count > 0)
                return urs[0];
            else
                return null;
        }
    }
}
