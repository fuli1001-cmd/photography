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
            return _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());
        }

        public async Task<List<PostViewModel>> GetLikedPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var likedPostIds = _postContext.UserPostRelations
                .Where(upr => upr.UserId.ToString() == userId && upr.UserPostRelationType == UserPostRelationType.Like)
                .Select(upr => upr.PostId);
            var likedPosts = _postContext.Posts.Where(p => likedPostIds.Contains(p.Id));
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(likedPosts);
            return _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());
        }

        public async Task<List<PostViewModel>> GetHotPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var availablePosts = GetAvailablePosts(userId).OrderByDescending(p => p.LikeCount);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(availablePosts);
            return _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());
        }

        public async Task<List<PostViewModel>> GetFollowedPostsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var availablePosts = GetAvailablePosts(userId);

            var followedUserIds = _postContext.UserRelations
                .Where(ur => ur.FollowerId.ToString() == userId)
                .Select(ur => ur.FollowedUserId);

            var followedUsersPosts = availablePosts.Where(p => followedUserIds.Contains(p.UserId)).OrderByDescending(p => p.Timestamp);

            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(followedUsersPosts);

            return _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());
        }

        public async Task<List<SameCityPostViewModel>> GetSameCityPostsAsync(string province, string city)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var availablePosts = GetAvailablePosts(userId);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(availablePosts);
            var sameCityPosts = (await postsWithNavigationProperties.ToListAsync()).Where(p => p.Province?.ToLower() == province.ToLower() && p.City?.ToLower() == city.ToLower());
            return _mapper.Map<List<SameCityPostViewModel>>(sameCityPosts);

            //var sameCityPosts = availablePosts.Where(p => p.Province.ToLower() == province.ToLower() && p.City.ToLower() == city.ToLower());
            //return await GetPostViewModelsAsync(sameCityPosts);
        }

        private IQueryable<Domain.AggregatesModel.PostAggregate.Post> GetAvailablePosts(string userId)
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

            // 公开和密码查看的帖子
            var otherPosts = _postContext.Posts.Where(p => p.Visibility != Domain.AggregatesModel.PostAggregate.Visibility.Friends && p.Visibility != Domain.AggregatesModel.PostAggregate.Visibility.SelectedFriends);

            return friendsViewPosts.Union(selectedFriendsViewPosts).Union(otherPosts);
        }

        //private async Task<List<PostViewModel>> GetPostViewModelsAsync(IQueryable<Domain.AggregatesModel.PostAggregate.Post> query)
        //{
        //    var posts = await query
        //        .Include(p => p.User)
        //        .Include(p => p.PostAttachments)
        //        .Include(p => p.ForwardedPost.User)
        //        .Include(p => p.ForwardedPost.PostAttachments)
        //        .Include(p => p.ForwardedPost.ForwardedPost)
        //        .ToListAsync();
        //    return _mapper.Map<List<PostViewModel>>(posts);
        //}

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
