using Microsoft.Extensions.Logging;
using Photography.WebApps.Management.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.WebApps.Management.Data
{
    public class PostService
    {
        private readonly PostHttpService _postHttpService;
        private readonly ILogger<PostService> _logger;

        public List<ViewModels.Post> Posts { get; private set; }

        public PostService(PostHttpService postHttpService, ILogger<PostService> logger)
        {
            _postHttpService = postHttpService ?? throw new ArgumentNullException(nameof(postHttpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ViewModels.Post>> GetUserPostsAsync(string userId, int pageNumber, int pageSize)
        {
            Posts = await _postHttpService.GetUserPostsAsync(userId, pageNumber, pageSize);
            return Posts;
        }
    }
}
