using Arise.DDD.Infrastructure;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<User, PostContext>, IUserRepository
    {
        public UserRepository(PostContext context) : base(context)
        {
            
        }

        //public async Task<Guid?> GetUserIdByCodeAsync(string code)
        //{
        //    if (code == null)
        //        return null;

        //    var userIds = await (from u in _context.Users
        //                         where u.Code.ToLower() == code.ToLower()
        //                         select u.Id).ToListAsync();

        //    if (userIds.Count > 0)
        //        return userIds[0];
        //    else
        //        return null;
        //}
    }
}
