using Arise.DDD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class UserShareRepository : EfRepository<UserShare, PostContext>, IUserShareRepository
    {
        public UserShareRepository(PostContext context) : base(context) { }

        public async Task<UserShare> GetUserShareAsync(Guid sharerId)
        {
            return await _context.UserShares.FirstOrDefaultAsync(s => s.SharerId == sharerId && s.PostId == null && s.PrivateTag == null);
        }

        public async Task<UserShare> GetUserShareAsync(Guid sharerId, Guid postId)
        {
            return await _context.UserShares.FirstOrDefaultAsync(s => s.SharerId == sharerId && s.PostId == postId && s.PrivateTag == null);
        }

        public async Task<UserShare> GetUserShareAsync(Guid sharerId, string privateTag)
        {
            return await _context.UserShares.FirstOrDefaultAsync(s => s.SharerId == sharerId && s.PrivateTag == privateTag && s.PostId == null);
        }
    }
}
