using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.CircleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class CircleRepository : EfRepository<Circle, PostContext>, ICircleRepository
    {
        public CircleRepository(PostContext context) : base(context)
        {

        }

        public async Task<Circle> GetCircleByNameAsync(string name)
        {
            return await _context.Circles.SingleOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<Circle> GetCircleWithPostsAsync(Guid circleId)
        {
            return await _context.Circles.Where(c => c.Id == circleId).Include(c => c.Posts).SingleOrDefaultAsync();
        }

        public async Task<int> GetUserCircleCount(Guid userId)
        {
            return await _context.Circles.Where(c => c.OwnerId == userId).CountAsync();
        }
    }
}
