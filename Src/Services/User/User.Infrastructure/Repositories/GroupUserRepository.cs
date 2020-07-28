using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class GroupUserRepository : EfRepository<GroupUser, UserContext>, IGroupUserRepository
    {
        public GroupUserRepository(UserContext context) : base(context)
        {

        }

        public async Task<GroupUser> GetGroupUserAsync(Guid groupId, Guid userId)
        {
            return await _context.GroupUsers.SingleOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);
        }

        public async Task<List<GroupUser>> GetGroupUsersByGroupIdAsync(Guid groupId)
        {
            return await _context.GroupUsers.Where(gu => gu.GroupId == groupId).ToListAsync();
        }
    }
}
