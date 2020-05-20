using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.UserAggregate;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using Photography.Services.Post.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<PostViewModel>> GetMyPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var myPosts = _postContext.Posts.Where(p => p.UserId.ToString() == userId);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(myPosts);
            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            // set liked status
            await SetLikedStatusAsync(posts, userId);

            return posts;
        }

        public async Task<List<PostViewModel>> GetLikedPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var likedPostIds = GetLikedPostIds(userId);
            var likedPosts = _postContext.Posts.Where(p => likedPostIds.Contains(p.Id));
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(likedPosts);
            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            // set liked status
            posts.ForEach(p => p.Liked = true);
            await SetFollowedStatusAsync(posts, userId);

            return posts;
        }

        public async Task<List<PostViewModel>> GetHotPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var availablePosts = GetAvailablePosts(userId).OrderByDescending(p => p.LikeCount);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(availablePosts);
            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            // set status
            await SetPropertiesAsync(posts, userId);

            return posts;
        }

        public async Task<List<PostViewModel>> GetFollowedPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var availablePosts = GetAvailablePosts(userId);

            var followedUserIds = GetFollowedUserIds(userId);

            var followedUsersPosts = availablePosts.Where(p => followedUserIds.Contains(p.UserId)).OrderByDescending(p => p.Timestamp);

            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(followedUsersPosts);

            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            // set status
            posts.ForEach(p => p.User.Followed = true);
            await SetLikedStatusAsync(posts, userId);

            return posts;
        }

        public async Task<List<PostViewModel>> GetSameCityPostsAsync(string cityCode)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var availablePosts = GetAvailablePosts(userId);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(availablePosts);
            var sameCityPosts = (await postsWithNavigationProperties.ToListAsync()).Where(p => p.CityCode?.ToLower() == cityCode.ToLower());
            var posts = _mapper.Map<List<PostViewModel>>(sameCityPosts);

            // set status
            await SetPropertiesAsync(posts, userId);

            return posts;

            //var sameCityPosts = availablePosts.Where(p => p.Province.ToLower() == province.ToLower() && p.City.ToLower() == city.ToLower());
            //return await GetPostViewModelsAsync(sameCityPosts);
        }

        private IQueryable<Domain.AggregatesModel.PostAggregate.Post> GetAvailablePosts(string userId)
        {
            // 公开和密码查看的帖子
            var otherPosts = _postContext.Posts.Where(p => p.Visibility != Domain.AggregatesModel.PostAggregate.Visibility.Friends && p.Visibility != Domain.AggregatesModel.PostAggregate.Visibility.SelectedFriends);

            if (!string.IsNullOrEmpty(userId))
            {
                // 朋友可见的帖子
                var friendsViewPosts = _postContext.Posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Friends);
                friendsViewPosts = friendsViewPosts.Where(p => GetFriendsIds(userId).Contains(p.UserId));

                // 指定朋友可见的帖子
                var selectedFriendsViewPosts = _postContext.Posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.SelectedFriends);
                selectedFriendsViewPosts = from p in selectedFriendsViewPosts
                                           join upr in _postContext.UserPostRelations
                                           on p.Id equals upr.PostId
                                           where upr.UserId.ToString() == userId && upr.UserPostRelationType == UserPostRelationType.View
                                           select p;

                return friendsViewPosts.Union(selectedFriendsViewPosts).Union(otherPosts);
            }
            else
                return otherPosts;
        }

        private IQueryable<Guid> GetLikedPostIds(string userId)
        {
            return _postContext.UserPostRelations
                .Where(upr => upr.UserId.ToString() == userId && upr.UserPostRelationType == UserPostRelationType.Like)
                .Select(upr => upr.PostId);
        }

        private IQueryable<Guid> GetFollowedUserIds(string userId)
        {
            return _postContext.UserRelations
                .Where(ur => ur.FollowerId.ToString() == userId)
                .Select(ur => ur.FollowedUserId);
        }

        private async Task SetPropertiesAsync(List<PostViewModel> posts, string userId)
        {
            var likedPostIds = await GetLikedPostIds(userId).ToListAsync();
            var followedUserIds = await GetFollowedUserIds(userId).ToListAsync();
            posts.ForEach(p =>
            {
                p.Liked = likedPostIds.Contains(p.Id) ? true : false;
                p.User.Followed = followedUserIds.Contains(p.User.Id) ? true : false;
            });
        }

        private async Task SetLikedStatusAsync(List<PostViewModel> posts, string userId)
        {
            var likedPostIds = await GetLikedPostIds(userId).ToListAsync();
            posts.ForEach(p => p.Liked = likedPostIds.Contains(p.Id) ? true : false);
        }

        private async Task SetFollowedStatusAsync(List<PostViewModel> posts, string userId)
        {
            var followedUserIds = await GetFollowedUserIds(userId).ToListAsync();
            posts.ForEach(p => p.User.Followed = followedUserIds.Contains(p.User.Id) ? true : false);
        }

        private IQueryable<Domain.AggregatesModel.PostAggregate.Post> GetPostsWithNavigationPropertiesAsync(IQueryable<Domain.AggregatesModel.PostAggregate.Post> query)
        {
            return query
                .Include(p => p.User)
                .Include(p => p.PostAttachments)
                .Include(p => p.ForwardedPost.User)
                .Include(p => p.ForwardedPost.PostAttachments)
                .Include(p => p.ForwardedPost.ForwardedPost);
        }

        private IQueryable<Guid> GetFriendsIds(string userId)
        {
            var friendsQuery = from ur1 in _postContext.UserRelations
                               join ur2 in _postContext.UserRelations on
                               new { C1 = ur1.FollowerId, C2 = ur1.FollowedUserId }
                               equals
                               new { C1 = ur2.FollowedUserId, C2 = ur2.FollowerId }
                               where ur1.FollowerId.ToString() == userId
                               select ur1.FollowedUserId;

            return friendsQuery;
        }
    }
}
