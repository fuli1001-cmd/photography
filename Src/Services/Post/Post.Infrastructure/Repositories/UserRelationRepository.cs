using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.UserRelationAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserRelationRepository : EfRepository<UserRelation, PostContext>, IUserRelationRepository
    {
        public UserRelationRepository(PostContext context) : base(context)
        {

        }

        public async Task<UserRelation> GetUserRelationAsync(Guid FollowerId, Guid FollowedUserId)
        {
            return await _context.UserRelations.SingleOrDefaultAsync(ur => ur.FollowedUserId == FollowedUserId && ur.FollowerId == FollowerId);
        }
    }
}
