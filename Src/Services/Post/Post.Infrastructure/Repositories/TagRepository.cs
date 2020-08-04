using Arise.DDD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Repositories
{
    public class TagRepository : EfRepository<Tag, PostContext>, ITagRepository
    {
        public TagRepository(PostContext context) : base(context)
        {

        }

        public async Task<List<Tag>> GetPublicTagsByNames(List<string> names)
        {
            names = names.Select(n => n.ToLower()).ToList();
            return await _context.Tags.Where(t => t.UserId == null && names.Contains(t.Name.ToLower())).ToListAsync();
        }

        public async Task<Tag> GetUserPrivateTagByName(Guid userId, string name)
        {
            return await _context.Tags.Where(t => t.UserId == userId && name.ToLower() == t.Name.ToLower()).SingleOrDefaultAsync();
        }

        public async Task<int> GetUserPrivateTagCount(Guid userId)
        {
            return await _context.Tags.Where(t => t.UserId == userId).CountAsync();
        }
    }
}
