using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate
{
    public interface IUserRelationRepository : IRepository<UserRelation>
    {
        Task<UserRelation> GetUserRelationAsync(Guid FollowerId, Guid FollowedUserId);
    }
}
