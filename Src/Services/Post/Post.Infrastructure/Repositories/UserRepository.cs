using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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
        public UserRepository(PostContext context) : base(context) { }

        public async Task<List<User>> GetUsersAsync(List<Guid> userIds)
        {
            return await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        }

        public async Task<List<Guid>> GetUserIdsByNicknameAsync(IEnumerable<string> nicknames)
        {
            if (nicknames == null)
                return new List<Guid>();

            return await _context.Users.Where(u => nicknames.Contains(u.Nickname)).Select(u => u.Id).ToListAsync();
        }
    }
}
