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
using Photography.Services.Post.API.Query.Extensions;
using Arise.DDD.API.Paging;

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

        /// <summary>
        /// 用户的帖子
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> GetUserPostsAsync(Guid userId, PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var queryableDto = from p in _postContext.Posts
                               join u in _postContext.Users
                               on p.UserId equals u.Id
                               where p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post && p.UserId == userId
                               select new PostViewModel
                               {
                                   Id = p.Id,
                                   Text = p.Text,
                                   LikeCount = p.LikeCount,
                                   ShareCount = p.ShareCount,
                                   CommentCount = p.CommentCount,
                                   CreatedTime = p.CreatedTime,
                                   UpdatedTime = p.UpdatedTime,
                                   Commentable = p.Commentable ?? true,
                                   ForwardType = p.ForwardType,
                                   ShareType = p.ShareType,
                                   ViewPassword = p.ViewPassword,
                                   ShowOriginalText = p.ShowOriginalText,
                                   Latitude = p.Latitude ?? 0,
                                   Longitude = p.Longitude ?? 0,
                                   LocationName = p.LocationName,
                                   Address = p.Address,
                                   Liked = (from upr in _postContext.UserPostRelations
                                            where upr.UserId == myId && upr.PostId == p.Id && upr.UserPostRelationType == UserPostRelationType.Like
                                            select upr.Id)
                                           .Any(),
                                   User = new PostUserViewModel
                                   {
                                       Id = u.Id,
                                       Nickname = u.Nickname,
                                       Avatar = u.Avatar,
                                       UserType = u.UserType,
                                       Followed = (from ur in _postContext.UserRelations
                                                   where ur.FollowerId == myId && ur.FollowedUserId == u.Id
                                                   select ur.Id)
                                                  .Any()
                                   },
                                   PostAttachments = from a in p.PostAttachments
                                                     select new PostAttachmentViewModel
                                                     {
                                                         Id = a.Id,
                                                         Name = a.Name,
                                                         Text = a.Text,
                                                         AttachmentType = a.AttachmentType
                                                     },
                                   ForwardedPost = p.ForwardedPostId == null ? null : new ForwardedPostViewModel
                                   {
                                       Id = p.ForwardedPost.Id,
                                       Text = p.ForwardedPost.Text,
                                       PostAttachments = from fa in p.ForwardedPost.PostAttachments
                                                         select new PostAttachmentViewModel
                                                         {
                                                             Id = fa.Id,
                                                             Name = fa.Name,
                                                             Text = fa.Text,
                                                             AttachmentType = fa.AttachmentType
                                                         }
                                   }
                               };

            var pagedDto = await PagedList<PostViewModel>.ToPagedListAsync(queryableDto, pagingParameters);

            pagedDto.ForEach(dto =>
            {
                 if (dto.ForwardedPost != null)
                     dto.ForwardedPost.User = (from fu in _postContext.Users
                                               join fp in _postContext.Posts
                                               on fu.Id equals fp.UserId
                                               where fp.Id == dto.ForwardedPost.Id
                                               select new BaseUserViewModel
                                               {
                                                   Id = fu.Id,
                                                   Nickname = fu.Nickname
                                               })
                                              .SingleOrDefault();
             });

            return pagedDto;

            //var userPosts = _postContext.Posts.Where(p => p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post && p.UserId == userId);
            //var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(userPosts);
            //var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            //await SetPropertiesAsync(posts, myId, false, myId == userId);

            //return posts;
        }

        /// <summary>
        /// 赞过的帖子
        /// </summary>
        /// <returns></returns>
        public async Task<List<PostViewModel>> GetLikedPostsAsync()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var likedPostIds = GetLikedPostIds(userId);
            var likedPosts = _postContext.Posts.Where(p => likedPostIds.Contains(p.Id) && p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(likedPosts);
            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            await SetPropertiesAsync(posts, userId, true, false);

            return posts;
        }

        public async Task<List<PostViewModel>> GetHotPostsAsync()
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);
            var availablePosts = GetAvailablePosts(userId).OrderByDescending(p => p.LikeCount);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(availablePosts);
            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            await SetPropertiesAsync(posts, userId, false, false);

            return posts;
        }

        public async Task<List<PostViewModel>> GetFollowedPostsAsync()
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var availablePosts = GetAvailablePosts(userId);

            var followedUserIds = GetFollowedUserIds(userId);

            var followedUsersPosts = availablePosts.Where(p => p.UserId == userId || followedUserIds.Contains(p.UserId)).OrderByDescending(p => p.CreatedTime);

            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(followedUsersPosts);

            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            await SetPropertiesAsync(posts, userId, false, true);

            return posts;
        }

        public async Task<List<PostViewModel>> GetSameCityPostsAsync(string cityCode)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var availablePosts = GetAvailablePosts(userId);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(availablePosts);

            if (!string.IsNullOrEmpty(cityCode))
                postsWithNavigationProperties = postsWithNavigationProperties.Where(p => p.CityCode != null && p.CityCode.ToLower() == cityCode.ToLower());

            var posts = _mapper.Map<List<PostViewModel>>(await postsWithNavigationProperties.ToListAsync());

            await SetPropertiesAsync(posts, userId, false, false);

            return posts;
        }

        private IQueryable<Domain.AggregatesModel.PostAggregate.Post> GetAvailablePosts(Guid userId)
        {
            var posts = _postContext.Posts.Where(p => p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post);

            // 公开、密码查看的帖子、以及自己发的帖子
            var otherPosts = posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Public 
                || p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Password
                || (userId != Guid.Empty && p.UserId == userId));

            if (userId != Guid.Empty)
            {
                // 朋友可见的帖子
                var friendsViewPosts = posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Friends);
                friendsViewPosts = friendsViewPosts.Where(p => GetFriendsIds(userId).Contains(p.UserId));

                // 指定朋友可见的帖子
                var selectedFriendsViewPosts = posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.SelectedFriends);
                selectedFriendsViewPosts = from p in selectedFriendsViewPosts
                                           join upr in _postContext.UserPostRelations
                                           on p.Id equals upr.PostId
                                           where upr.UserId == userId && upr.UserPostRelationType == UserPostRelationType.View
                                           select p;

                return friendsViewPosts.Union(selectedFriendsViewPosts).Union(otherPosts);
            }
            else
                return otherPosts;
        }

        private IQueryable<Guid> GetLikedPostIds(Guid userId)
        {
            return _postContext.UserPostRelations
                .Where(upr => upr.UserId == userId && upr.UserPostRelationType == UserPostRelationType.Like)
                .Select(upr => upr.PostId.Value);
        }

        private IQueryable<Guid> GetFollowedUserIds(Guid userId)
        {
            return _postContext.UserRelations
                .Where(ur => ur.FollowerId == userId)
                .Select(ur => ur.FollowedUserId);
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

        private IQueryable<Guid> GetFriendsIds(Guid userId)
        {
            var friendsQuery = from ur1 in _postContext.UserRelations
                               join ur2 in _postContext.UserRelations on
                               new { C1 = ur1.FollowerId, C2 = ur1.FollowedUserId }
                               equals
                               new { C1 = ur2.FollowedUserId, C2 = ur2.FollowerId }
                               where ur1.FollowerId == userId
                               select ur1.FollowedUserId;

            return friendsQuery;
        }

        private async Task SetPropertiesAsync(List<PostViewModel> posts, Guid userId, bool allLiked, bool allFollowed)
        {
            List<Guid> likedPostIds = null;
            List<Guid> followedUserIds = null;

            if (!allLiked)
                likedPostIds = await GetLikedPostIds(userId).ToListAsync();

            if (!allFollowed)
                followedUserIds = await GetFollowedUserIds(userId).ToListAsync();

            posts.ForEach(p =>
            {
                p.Liked = allLiked ? true : (likedPostIds.Contains(p.Id) ? true : false);
                p.User.Followed = allFollowed ? true : (followedUserIds.Contains(p.User.Id) ? true : false);
                p.SetAttachmentProperties(_logger);
            });
        }

        public async Task<PostViewModel> GetPostAsync(Guid postId)
        {
            var posts = _postContext.Posts.Where(p => p.Id == postId);
            var postsWithNavigationProperties = GetPostsWithNavigationPropertiesAsync(posts);
            return _mapper.Map<PostViewModel>(await postsWithNavigationProperties.SingleOrDefaultAsync());
        }

        public async Task<List<PostViewModel>> SearchPosts(string key, string cityCode)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);
            
            var query = GetAvailablePosts(userId);
            query = GetPostsWithNavigationPropertiesAsync(query);

            if (!string.IsNullOrEmpty(key))
            {
                key = key.ToLower();
                query = from p in query
                        where p.User.Nickname.ToLower().Contains(key) || p.Text.ToLower().Contains(key)
                        select p;
            }

            if (!string.IsNullOrEmpty(cityCode))
                query = query.Where(p => p.CityCode != null && p.CityCode.ToLower() == cityCode.ToLower());

            var posts = _mapper.Map<List<PostViewModel>>(await query.OrderByDescending(p => p.CreatedTime).ToListAsync());

            await SetPropertiesAsync(posts, userId, false, false);

            return posts;
        }
    }
}
