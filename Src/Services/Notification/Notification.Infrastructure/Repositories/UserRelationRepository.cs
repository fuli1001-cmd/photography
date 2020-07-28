using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Notification.Domain.AggregatesModel.UserRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class UserRelationRepository : EfRepository<UserRelation, NotificationContext>, IUserRelationRepository
    {
        public UserRelationRepository(NotificationContext context) : base(context) { }

        public async Task<UserRelation> GetUserRelationAsync(Guid followerId, Guid followedUserId)
        {
            return await _context.UserRelations.FirstOrDefaultAsync(ur => ur.FollowerId == followerId && ur.FollowedUserId == followedUserId);
        }
    }
}
