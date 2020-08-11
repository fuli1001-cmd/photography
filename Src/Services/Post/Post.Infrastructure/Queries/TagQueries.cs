using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Infrastructure.Queries
{
    public class TagQueries : ITagQueries
    {
        private readonly PostContext _dbContext;
        private readonly ILogger<TagQueries> _logger;

        public TagQueries(PostContext dbContext, ILogger<TagQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 常用公共标签
        public async Task<IEnumerable<string>> GetPopularPublicTagsAsync()
        {
            return await _dbContext.Tags.Where(t => t.TagType == TagType.Public).OrderByDescending(t => t.Count).ThenByDescending(t => t.CreatedTime).Take(10).Select(t => t.Name).ToListAsync();
        }

        // 系统标签
        public async Task<IEnumerable<string>> GetSystemTagsAsync()
        {
            return await _dbContext.Tags.Where(t => t.TagType == TagType.System).OrderBy(t => t.Index).Select(t => t.Name).ToListAsync();
        }

        // 用户的私有标签（帖子类别）
        public async Task<IEnumerable<string>> GetUserPrivateTagsAsync(Guid userId)
        {
            return await _dbContext.Tags.Where(t => t.UserId == userId).Select(t => t.Name).ToListAsync();
        }
    }
}
