using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Domain.AggregatesModel.GroupAggregate
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<Group> GetGroupWithMembersAsync(Guid groupId);
    }
}
