using Arise.DDD.Infrastructure.Data;
using Photography.Services.Notification.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Photography.Services.Notification.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<User, NotificationContext>, IUserRepository
    {
        public UserRepository(NotificationContext context) : base(context) { }

        public async Task<string> GetNickNameAsync(Guid userId)
        {
            return await (from u in _context.Users
                          where u.Id == userId
                          select u.Nickname)
                          .FirstOrDefaultAsync();
        }
    }
}
