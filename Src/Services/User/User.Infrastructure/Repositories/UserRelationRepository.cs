using Arise.DDD.Infrastructure.Data;
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

        public async Task<UserRelation> GetAsync(Guid fromUserId, Guid toUserId)
        {
            return await _context.UserRelations.FirstOrDefaultAsync(ur => ur.FromUserId == fromUserId && ur.ToUserId == toUserId);
        }

        //public async Task<IEnumerable<UserRelation>> GetFriendsAsync(Guid myId)
        //{
        //    return await (from ur1 in _context.UserRelations
        //           join ur2 in _context.UserRelations
        //           on new { FollowerId = ur1.FollowerId, FollowedUserId = ur1.FollowedUserId } equals new { FollowerId = ur2.FollowedUserId, FollowedUserId = ur2.FollowerId }
        //           where ur1.FollowerId == myId
        //           select ur1).ToListAsync();
        //}
    }
}
