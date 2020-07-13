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
        private readonly UserHttpService _userHttpService;
        private readonly ILogger<PostService> _logger;

        public PagedResponseWrapper<List<Post>> PagedData { get; private set; }

        public PostService(PostHttpService postHttpService, UserHttpService userHttpService, ILogger<PostService> logger)
        {
            _postHttpService = postHttpService ?? throw new ArgumentNullException(nameof(postHttpService));
            _userHttpService = userHttpService ?? throw new ArgumentNullException(nameof(userHttpService));
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
                PagedData = new PagedResponseWrapper<List<Post>> { Data = new List<Post>(), PagingInfo = new PagingInfo() };
                return PagedData;
            }
        }

        public async Task<bool> DeletePostAsync(Post post)
        {
            var tasks = new List<Task<bool>>();

            try
            {
                var delPostTask = _postHttpService.DeletePostAsync(post);
                tasks.Add(delPostTask);

                // 如果是公共可见的帖子，删贴后将用户禁用
                if (post.Visibility == Visibility.Public)
                    tasks.Add(_userHttpService.DisableUserAsync(post.User, true));

                await Task.WhenAll(tasks);

                return await delPostTask;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeletePostAsync failed, {@Exception}", ex);
                return false;
            }
        }

        public async Task<PagedResponseWrapper<List<Post>>> GetAppointmentsAsync(int pageNumber, int pageSize)
        {
            try
            {
                PagedData = await _postHttpService.GetAppointsAsync(pageNumber, pageSize);
                return PagedData;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAppointmentsAsync failed, {@Exception}", ex);
                PagedData = new PagedResponseWrapper<List<Post>> { Data = new List<Post>(), PagingInfo = new PagingInfo() };
                return PagedData;
            }
        }

        public async Task<bool> DeleteAppointmentAsync(Post post)
        {
            var tasks = new List<Task<bool>>();

            try
            {
                var delPostTask = _postHttpService.DeleteAppointmentAsync(post);
                tasks.Add(delPostTask);

                // 删除约拍后将用户禁用
                tasks.Add(_userHttpService.DisableUserAsync(post.User, true));

                await Task.WhenAll(tasks);

                return await delPostTask;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteAppointmentAsync failed, {@Exception}", ex);
                return false;
            }
        }
    }
}
