using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Domain.AggregatesModel.GroupUserAggregate
{
    public interface IGroupUserRepository : IRepository<GroupUser>
    {
        Task<List<GroupUser>> GetGroupUsersByGroupIdAsync(Guid groupId);

        Task<GroupUser> GetGroupUserAsync(Guid groupId, Guid userId);
    }
}
