using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Photography.Services.Post.API.Query.Extensions;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Infrastructure.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.EF
{
    public class PostQueries : IPostQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<PostQueries> _logger;

        public PostQueries(PostContext postContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<PostQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PostViewModel>> GetPostsAsync(PostType postType)
        {
            if (postType == PostType.Hot)
                return await GetHotPostsAsync();
            else if (postType == PostType.Followed)
                return await GetFollowedPostsAsync();
            else
                return null;
        }

        private async Task<List<PostViewModel>> GetHotPostsAsync()
        {
            return await GetPostViewModelsAsync(_postContext.Posts);
        }

        private async Task<List<PostViewModel>> GetFollowedPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var followedUserIds = _postContext.UserRelations
                .Where(ur => ur.FollowerId.ToString() == userId)
                .Select(ur => ur.FollowedUserId);

            var queryablePosts = _postContext.Posts.Where(p => followedUserIds.Contains(p.UserId));

            return await GetPostViewModelsAsync(queryablePosts);
        }

        //private IQueryable<Domain.AggregatesModel.PostAggregate.Post> FilterByVisibility(IQueryable<Domain.AggregatesModel.PostAggregate.Post> query)
        //{
            
        //}

        private async Task<List<PostViewModel>> GetPostViewModelsAsync(IQueryable<Domain.AggregatesModel.PostAggregate.Post> query)
        {
            var postViewModels = await query
                .Include(p => p.User)
                .Include(p => p.PostAttachments)
                .Include(p => p.ForwardedPost.User)
                .Include(p => p.ForwardedPost.PostAttachments)
                .Include(p => p.ForwardedPost.ForwardedPost)
                .OrderByDescending(p => p.Points)
                .ToListAsync();
            return _mapper.Map<List<PostViewModel>>(postViewModels);
        }
    }
}
