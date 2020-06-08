using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.AggregatesModel.GroupAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Photography.Services.User.Infrastructure.Repositories
{
    public class GroupRepository : EfRepository<Domain.AggregatesModel.GroupAggregate.Group, UserContext>, IGroupRepository
    {
        public GroupRepository(UserContext context) : base(context)
        {

        }

        public async Task<Domain.AggregatesModel.GroupAggregate.Group> GetGroupWithMembersAsync(Guid groupId)
        {
            return await _context.Groups.Where(g => g.Id == groupId).Include(g => g.GroupUsers).SingleOrDefaultAsync();
        }
    }
}
