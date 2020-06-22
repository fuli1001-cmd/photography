using Microsoft.Extensions.Logging;
using Photography.WebApps.Management.Services;
using Photography.WebApps.Management.ViewModels;
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

        public PagedResponseWrapper<List<Post>> PagedData { get; private set; }

        public PostService(PostHttpService postHttpService, ILogger<PostService> logger)
        {
            _postHttpService = postHttpService ?? throw new ArgumentNullException(nameof(postHttpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedResponseWrapper<List<Post>>> GetPostsAsync(int pageNumber, int pageSize)
        {
            try
            {
                PagedData = await _postHttpService.GetPostsAsync(pageNumber, pageSize);
                return PagedData;
            }
            catch(Exception ex)
            {
                _logger.LogError("GetPostsAsync failed, {@Exception}", ex);
                return new PagedResponseWrapper<List<Post>>(); 
            }
        }

        public async Task<bool> DeletePostAsync(Post post)
        {
            try
            {
                return await _postHttpService.DeletePostAsync(post);
            }
            catch (Exception ex)
            {
                _logger.LogError("DeletePostAsync failed, {@Exception}", ex);
                return false;
            }
        }
    }
}
