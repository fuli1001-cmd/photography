using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.Domain.AggregatesModel.TagAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class TagQueries : ITagQueries
    {
        private readonly PostContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TagQueries> _logger;

        public TagQueries(PostContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<TagQueries> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 常用公共标签
        public async Task<IEnumerable<string>> GetPopularPublicTagsAsync()
        {
            return await _dbContext.Tags.Where(t => t.UserId == null).OrderByDescending(t => t.Count).Take(10).Select(t => t.Name).ToListAsync();
        }

        // 用户的私有标签（帖子类别）
        public async Task<IEnumerable<string>> GetUserPrivateTagsAsync(Guid userId)
        {
            return await _dbContext.Tags.Where(t => t.UserId == userId).Select(t => t.Name).ToListAsync();
        }
    }
}
