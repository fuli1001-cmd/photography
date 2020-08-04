using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Domain.AggregatesModel.UserRelationAggregate
{
    public interface IUserRelationRepository : IRepository<UserRelation>
    {
        Task<UserRelation> GetAsync(Guid fromUserId, Guid toUserId);

        //Task<IEnumerable<UserRelation>> GetFriendsAsync(Guid myId);
    }
}
