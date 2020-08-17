using Microsoft.AspNetCore.Http;
using Photography.Services.Post.API.Query.Interfaces;
using Photography.Services.Post.API.Query.ViewModels;
using Photography.Services.Post.Domain.AggregatesModel.UserPostRelationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Photography.Services.Post.API.Query.Extensions;
using Arise.DDD.API.Paging;
using Arise.DDD.Domain.Exceptions;
using Photography.Services.Post.Infrastructure.Queries.Models;
using System.Text;

namespace Photography.Services.Post.Infrastructure.Queries
{
    public class PostQueries : IPostQueries
    {
        private readonly PostContext _postContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PostQueries> _logger;

        public PostQueries(PostContext postContext, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<PostQueries> logger)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 用户的帖子
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="privateTag">帖子类别</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> GetUserPostsAsync(Guid userId, string privateTag, string key, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            IQueryable<Domain.AggregatesModel.PostAggregate.Post> queryablePosts;
            if (myId != Guid.Empty && myId == userId)
            {
                // 查看自己的帖子时，返回所有的自己的帖子
                queryablePosts = _postContext.Posts.Where(p => p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post);
            }
            else
            {
                // 查看别人的帖子时，只返回公开的帖子、指定的朋友（或密码查看，密码查看默认是所有朋友查看）包含我可看的帖子
                queryablePosts = GetAvailablePosts(myId);
            }

            // 如果帖子种类不为空， 按帖子种类筛选一下
            if (!string.IsNullOrWhiteSpace(privateTag))
            {
                // “未分类”未系统保留关键字，表示帖子不属于任何类别
                if (privateTag == "未分类")
                    queryablePosts = queryablePosts.Where(p => p.PrivateTag == null || p.PrivateTag == "");
                else
                    queryablePosts = queryablePosts.Where(p => p.PrivateTag != null && p.PrivateTag.ToLower() == privateTag.ToLower());
            }

            // 有搜索关键字时，搜索昵称、文案和标签
            if (!string.IsNullOrWhiteSpace(key))
            {
                key = key.ToLower();
                queryablePosts = from p in queryablePosts
                                 where p.User.Nickname.ToLower().Contains(key) 
                                 || (p.Text != null && p.Text.ToLower().Contains(key)) 
                                 || (p.PublicTags != null && p.PublicTags.ToLower().Contains(key))
                                 select p;
            }

            var queryableUserPosts = from p in queryablePosts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     where p.UserId == userId
                                     select new UserPost { Post = p, User = u };

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        /// <summary>
        /// 赞过的帖子
        /// </summary>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> GetLikedPostsAsync(PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // 先获得可以查看的帖子
            var queryablePosts = GetAvailablePosts(myId);

            // 再筛选出其中赞过的帖子
            var queryableUserPosts = from p in queryablePosts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     join upr in _postContext.UserPostRelations
                                     on new { PostId = p.Id, UserId = myId, Type = UserPostRelationType.Like } equals new { PostId = upr.PostId.Value, UserId = upr.UserId.Value, Type = upr.UserPostRelationType }
                                     where p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post
                                     select new UserPost { Post = p, User = u };

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        /// <summary>
        /// 热门帖子
        /// </summary>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> GetHotPostsAsync(PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryablePosts = GetAvailablePosts(myId);

            var queryableUserPosts = GetAvailableUserPosts(queryablePosts).OrderByDescending(up => up.Post.Score).ThenByDescending(up => up.Post.UpdatedTime);

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        /// <summary>
        /// 关注的用户所发的帖子
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> GetFollowedPostsAsync(PagingParameters pagingParameters)
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var followedUserIds = GetFollowedUserIds(myId);

            var queryablePosts = GetAvailablePosts(myId).Where(p => p.UserId == myId || followedUserIds.Contains(p.UserId));

            var queryableUserPosts = GetAvailableUserPosts(queryablePosts);

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        /// <summary>
        /// 同城用户所发的帖子
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> GetSameCityPostsAsync(string cityCode, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryablePosts = GetAvailablePosts(myId);

            if (!string.IsNullOrWhiteSpace(cityCode))
                queryablePosts = queryablePosts.Where(p => p.CityCode != null && p.CityCode.ToLower() == cityCode.ToLower());

            var queryableUserPosts = GetAvailableUserPosts(queryablePosts);

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        // 获取系统标签下的帖子
        public async Task<PagedList<PostViewModel>> GetPostsBySystemTag(string systemTag, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryablePosts = GetAvailablePosts(myId).Where(p => p.SystemTag == systemTag);
            var queryableUserPosts = GetAvailableUserPosts(queryablePosts);
            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        /// <summary>
        /// 获取指定id列表的帖子
        /// </summary>
        /// <param name="postIds"></param>
        /// <returns></returns>
        public async Task<List<PostViewModel>> GetPostsAsync(List<Guid> postIds)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     where postIds.Contains(p.Id)
                                     select new UserPost { Post = p, User = u };

            var postViewModels = await GetQueryablePostViewModels(queryableUserPosts, myId).ToListAsync();

            postViewModels.ForEach(p => SetAttachment(p));

            return postViewModels;
        }

        /// <summary>
        /// 帖子详情
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<PostViewModel> GetPostAsync(Guid postId)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     where p.Id == postId
                                     select new UserPost { Post = p, User = u };

            var postViewModel = await GetQueryablePostViewModels(queryableUserPosts, myId).SingleOrDefaultAsync();

            SetAttachment(postViewModel);

            return postViewModel;
        }

        /// <summary>
        /// 分享的帖子的详情
        /// 与帖子详情的区别在于需要检查事件限制，超过时间限制不予返回
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<PostViewModel> GetSharedPostAsync(Guid postId, Guid sharedUserId)
        {
            var sharedUser = await _postContext.Users.FirstOrDefaultAsync(u => u.Id == sharedUserId);
            if (sharedUser == null)
                throw new ClientException("操作失败", new List<string> { $"User {sharedUserId} does not exist." });

            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     where p.Id == postId && p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post
                                     select new UserPost { Post = p, User = u };

            var postViewModel = await GetQueryablePostViewModels(queryableUserPosts, myId).SingleOrDefaultAsync();

            SetAttachment(postViewModel);

            return postViewModel;
        }

        public async Task<PagedList<PostViewModel>> GetSharedPostsAsync(string privateTag, Guid sharedUserId, string key, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     where p.UserId == sharedUserId 
                                     && p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post
                                     && p.PostAuthStatus == Domain.AggregatesModel.PostAggregate.PostAuthStatus.Authenticated
                                     select new UserPost { Post = p, User = u };

            if (privateTag == "未分类")
                queryableUserPosts = queryableUserPosts.Where(up => up.Post.PrivateTag == null || up.Post.PrivateTag == "");
            else
                queryableUserPosts = queryableUserPosts.Where(up => up.Post.PrivateTag == privateTag);

            if (!string.IsNullOrWhiteSpace(key))
            {
                key = key.ToLower();
                queryableUserPosts = from up in queryableUserPosts
                                     where up.User.Nickname.ToLower().Contains(key)
                                     || (up.Post.Text != null && up.Post.Text.ToLower().Contains(key))
                                     || (up.Post.PublicTags != null && up.Post.PublicTags.ToLower().Contains(key))
                                     || (up.Post.ForwardedPost != null && up.Post.ForwardedPost.User.Nickname.ToLower().Contains(key))
                                     || (up.Post.ShowOriginalText != null && up.Post.ShowOriginalText.Value && up.Post.ForwardedPost != null && up.Post.ForwardedPost.Text != null && up.Post.ForwardedPost.Text.ToLower().Contains(key))
                                     || (up.Post.ForwardedPost != null && up.Post.ForwardedPost.PublicTags != null && up.Post.ForwardedPost.PublicTags.ToLower().Contains(key))
                                     select up;
            }

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            var result = await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);

            result.ForEach(p => SetAttachment(p));

            return result;
        }

        public async Task<PagedList<PostViewModel>> GetSharedPostsAsync(Guid sharedUserId, string key, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            var queryableUserPosts = from p in _postContext.Posts
                                     join u in _postContext.Users
                                     on p.UserId equals u.Id
                                     where p.UserId == sharedUserId 
                                     && p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post
                                     && p.PostAuthStatus == Domain.AggregatesModel.PostAggregate.PostAuthStatus.Authenticated
                                     select new UserPost { Post = p, User = u };

            if (!string.IsNullOrWhiteSpace(key))
            {
                key = key.ToLower();
                queryableUserPosts = from up in queryableUserPosts
                                     where up.User.Nickname.ToLower().Contains(key)
                                     || (up.Post.Text != null && up.Post.Text.ToLower().Contains(key))
                                     || (up.Post.PublicTags != null && up.Post.PublicTags.ToLower().Contains(key))
                                     || (up.Post.ForwardedPost != null && up.Post.ForwardedPost.User.Nickname.ToLower().Contains(key))
                                     || (up.Post.ShowOriginalText != null && up.Post.ShowOriginalText.Value && up.Post.ForwardedPost != null && up.Post.ForwardedPost.Text != null && up.Post.ForwardedPost.Text.ToLower().Contains(key))
                                     || (up.Post.ForwardedPost != null && up.Post.ForwardedPost.PublicTags != null && up.Post.ForwardedPost.PublicTags.ToLower().Contains(key))
                                     select up;
            }

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            var result = await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);

            result.ForEach(p => SetAttachment(p));

            return result;
        }

        /// <summary>
        /// 搜索发帖者昵称、帖子文案、帖子标签包含关键字的帖子
        /// </summary>
        /// <param name="key">搜索关键字</param>
        /// <param name="cityCode">城市代码，若指定了城市代码，则只在属于该城市的帖子中搜索</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        public async Task<PagedList<PostViewModel>> SearchPosts(string key, string cityCode, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            
            IQueryable<Domain.AggregatesModel.PostAggregate.Post> queryablePosts = null;

            if (role == "admin")
                queryablePosts = _postContext.Posts.Where(p => p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post);
            else
                queryablePosts = GetAvailablePosts(myId);

            // 有搜索关键字时，搜索昵称、文案和标签
            if (!string.IsNullOrWhiteSpace(key))
            {
                key = key.ToLower();
                queryablePosts = from p in queryablePosts
                                 where p.User.Nickname.ToLower().Contains(key)
                                 || (p.Text != null && p.Text.ToLower().Contains(key))
                                 || (p.PublicTags != null && p.PublicTags.ToLower().Contains(key))
                                 select p;
            }

            // 搜索城市代码
            if (!string.IsNullOrEmpty(cityCode))
                queryablePosts = queryablePosts.Where(p => p.CityCode != null && p.CityCode.ToLower() == cityCode.ToLower());

            var queryableUserPosts = GetAvailableUserPosts(queryablePosts);

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        public async Task<PagedList<PostViewModel>> GetPostsByPublicTagAsync(string tag, PagingParameters pagingParameters)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                _logger.LogError("GetPostsByPublicTagAsync: tag is empty.");
                throw new ClientException("操作失败", new List<string> { "Can't get posts by empty tag" });
            }

            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            // 可见的帖子
            var queryablePosts = GetAvailablePosts(myId);

            // 具有该标签的帖子
            queryablePosts = queryablePosts.Where(p => p.PublicTags != null && p.PublicTags.ToLower().Contains(tag.ToLower()));

            var queryableUserPosts = GetAvailableUserPosts(queryablePosts);

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId).OrderByDescending(dto => dto.UpdatedTime);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        public async Task<PagedList<PostViewModel>> GetCirclePostsAsync(Guid circleId, bool onlyGood, string key, string sortBy, PagingParameters pagingParameters)
        {
            var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var myId = claim == null ? Guid.Empty : Guid.Parse(claim.Value);

            // 可见的帖子
            var queryablePosts = GetAvailablePosts(myId);

            // 圈子的帖子
            queryablePosts = queryablePosts.Where(p => p.CircleId == circleId);

            // 只查询精华帖
            if (onlyGood)
                queryablePosts = queryablePosts.Where(p => p.CircleGood);

            // 有搜索关键字时，搜索昵称、文案和标签
            if (!string.IsNullOrWhiteSpace(key))
            {
                key = key.ToLower();
                queryablePosts = from p in queryablePosts
                                 where p.User.Nickname.ToLower().Contains(key)
                                 || (p.Text != null && p.Text.ToLower().Contains(key))
                                 || (p.PublicTags != null && p.PublicTags.ToLower().Contains(key))
                                 select p;
            }

            if (!string.IsNullOrWhiteSpace(sortBy) && sortBy.ToLower() == "score")
                queryablePosts = queryablePosts.OrderByDescending(p => p.Score).ThenByDescending(p => p.UpdatedTime);
            else
                queryablePosts = queryablePosts.OrderByDescending(p => p.UpdatedTime);

            var queryableUserPosts = GetAvailableUserPosts(queryablePosts);

            var queryableDto = GetQueryablePostViewModels(queryableUserPosts, myId);

            return await GetPagedPostViewModelsAsync(queryableDto, pagingParameters);
        }

        private IQueryable<Domain.AggregatesModel.PostAggregate.Post> GetAvailablePosts(Guid myId)
        {
            var posts = _postContext.Posts.Where(p => p.PostType == Domain.AggregatesModel.PostAggregate.PostType.Post 
                && p.PostAuthStatus == Domain.AggregatesModel.PostAggregate.PostAuthStatus.Authenticated);

            // 公开、以及自己发的帖子
            var otherPosts = posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Public 
                || (myId != Guid.Empty && p.UserId == myId));

            if (myId != Guid.Empty)
            {
                // 朋友可见的帖子
                // 密码可见的帖子默认只能朋友看
                var friendsOrPasswordViewPosts = posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Friends || p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.Password);
                friendsOrPasswordViewPosts = friendsOrPasswordViewPosts.Where(p => GetFriendsIds(myId).Contains(p.UserId));

                // 指定朋友可见的帖子
                var selectedFriendsViewPosts = posts.Where(p => p.Visibility == Domain.AggregatesModel.PostAggregate.Visibility.SelectedFriends);
                selectedFriendsViewPosts = from p in selectedFriendsViewPosts
                                           join upr in _postContext.UserPostRelations
                                           on p.Id equals upr.PostId
                                           where upr.UserId == myId && upr.UserPostRelationType == UserPostRelationType.View
                                           select p;

                return friendsOrPasswordViewPosts.Union(selectedFriendsViewPosts).Union(otherPosts);
            }
            else
                return otherPosts;
        }

        private IQueryable<UserPost> GetAvailableUserPosts(IQueryable<Domain.AggregatesModel.PostAggregate.Post> queryablePosts)
        {
            return from p in queryablePosts
                   join u in _postContext.Users
                   on p.UserId equals u.Id
                   select new UserPost { Post = p, User = u };
        }

        private IQueryable<Guid> GetFollowedUserIds(Guid userId)
        {
            return _postContext.UserRelations
                .Where(ur => ur.FollowerId == userId)
                .Select(ur => ur.FollowedUserId);
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

        private IQueryable<PostViewModel> GetQueryablePostViewModels(IQueryable<UserPost> queryableUserPosts, Guid myId)
        {
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            return from up in queryableUserPosts
                   select new PostViewModel
                   {
                       Id = up.Post.Id,
                       Text = up.Post.Text,
                       LikeCount = up.Post.LikeCount,
                       ShareCount = up.Post.ShareCount + up.Post.ForwardCount,
                       CommentCount = up.Post.CommentCount,
                       CreatedTime = up.Post.CreatedTime,
                       UpdatedTime = up.Post.UpdatedTime,
                       Commentable = up.Post.Commentable ?? true,
                       ForwardType = up.Post.ForwardType,
                       ShareType = up.Post.ShareType,
                       ViewPassword = up.Post.ViewPassword,
                       ShowOriginalText = up.Post.ShowOriginalText,
                       Latitude = up.Post.Latitude ?? 0,
                       Longitude = up.Post.Longitude ?? 0,
                       LocationName = up.Post.LocationName,
                       Address = up.Post.Address,
                       CityCode = up.Post.CityCode,
                       Visibility = up.Post.Visibility,
                       SystemTag = up.Post.SystemTag,
                       PublicTags = up.Post.PublicTags,
                       PrivateTag = up.Post.PrivateTag,
                       CircleGood = up.Post.CircleGood,
                       PostAuthStatus = up.Post.PostAuthStatus,
                       Circle = up.Post.Circle == null ? null : new PostCircleViewModel
                       { 
                           Id = up.Post.Circle.Id, 
                           Name = up.Post.Circle.Name,
                           OwnerId = up.Post.Circle.OwnerId
                       },
                       FriendIds = from upr in _postContext.UserPostRelations
                                   where upr.PostId == up.Post.Id && upr.UserPostRelationType == UserPostRelationType.View
                                   select upr.UserId.Value,
                       Liked = (from upr in _postContext.UserPostRelations
                                where upr.UserId == myId && upr.PostId == up.Post.Id && upr.UserPostRelationType == UserPostRelationType.Like
                                select upr.Id)
                                .Any(),
                       User = new PostUserViewModel
                       {
                           Id = up.User.Id,
                           Nickname = up.User.Nickname,
                           Avatar = up.User.Avatar,
                           UserType = up.User.UserType,
                           OrgAuthStatus = up.User.OrgAuthStatus,
                           RealNameRegistrationStatus = up.User.IdAuthenticated ? IdAuthStatus.Authenticated : IdAuthStatus.NotAuthenticated,
                           Followed = (from ur in _postContext.UserRelations
                                       where ur.FollowerId == myId && ur.FollowedUserId == up.User.Id
                                       select ur.Id)
                                       .Any()
                       },
                       // 非私有附件、用户自己的帖子、或者管理员
                       PostAttachments = from a in up.Post.PostAttachments
                                         where !a.IsPrivate || up.Post.UserId == myId || role == "admin"
                                         select new PostAttachmentViewModel
                                         {
                                             Id = a.Id,
                                             Name = a.Name,
                                             Text = a.Text,
                                             AttachmentType = a.AttachmentType,
                                             IsPrivate = a.IsPrivate
                                         },
                       ForwardedPost = up.Post.ForwardedPostId == null ? null : new ForwardedPostViewModel
                       {
                           Id = up.Post.ForwardedPost.Id,
                           Text = up.Post.ForwardedPost.Text,
                           PublicTags = up.Post.ForwardedPost.PublicTags,
                           User = new BaseUserViewModel
                           {
                               Id = up.Post.ForwardedPost.User.Id,
                               Nickname = up.Post.ForwardedPost.User.Nickname
                           },
                           PostAttachments = from fa in up.Post.ForwardedPost.PostAttachments
                                             where !fa.IsPrivate || up.Post.ForwardedPost.UserId == myId || role == "admin"
                                             select new PostAttachmentViewModel
                                             {
                                                 Id = fa.Id,
                                                 Name = fa.Name,
                                                 Text = fa.Text,
                                                 AttachmentType = fa.AttachmentType,
                                                 IsPrivate = fa.IsPrivate
                                             }
                       }
                   };
        }

        private async Task<PagedList<PostViewModel>> GetPagedPostViewModelsAsync(IQueryable<PostViewModel> queryableDto, PagingParameters pagingParameters)
        {
            var pagedDto = await PagedList<PostViewModel>.ToPagedListAsync(queryableDto, pagingParameters);

            #region 设置转发帖子所属的用户
            //注：理论上应该可以在queryableDto的查询中设置，但是我始终得到错误，因此这里先取得pagedDto再来设置
            List<Guid> forwardPostIds = new List<Guid>();
            pagedDto.ForEach(dto =>
            {
                if (dto.ForwardedPost != null)
                    forwardPostIds.Add(dto.ForwardedPost.Id);
            });

            var userPosts = await (from fu in _postContext.Users
                                   join fp in _postContext.Posts
                                   on fu.Id equals fp.UserId
                                   where forwardPostIds.Contains(fp.Id)
                                   select new UserPost { Post = fp, User = fu })
                                   .ToListAsync();

            pagedDto.ForEach(dto =>
            {
                if (dto.ForwardedPost != null)
                {
                    dto.ForwardedPost.User = (from up in userPosts
                                              where up.Post.Id == dto.ForwardedPost.Id
                                              select new BaseUserViewModel
                                              {
                                                  Id = up.User.Id,
                                                  Nickname = up.User.Nickname
                                              })
                                             .SingleOrDefault();
                }

                SetAttachment(dto);
            });
            #endregion

            return pagedDto;
        }

        /// <summary>
        /// 设置附件的宽高和缩略图
        /// </summary>
        /// <param name="postViewModel"></param>
        private void SetAttachment(PostViewModel postViewModel)
        {
            if (postViewModel != null)
            {
                // set ForwardedPost attachment width and height
                if (postViewModel.ForwardedPost != null)
                {
                    foreach (var attachment in postViewModel.ForwardedPost.PostAttachments)
                        attachment.SetProperties();
                }

                // set attachment width and height
                foreach (var attachment in postViewModel.PostAttachments)
                    attachment.SetProperties();
            }
        }

        public async Task<PagedList<Domain.AggregatesModel.PostAggregate.Post>> GetPostsAsync(PagingParameters pagingParameters)
        {
            return await PagedList<Domain.AggregatesModel.PostAggregate.Post>.ToPagedListAsync(_postContext.Posts.OrderBy(p => p.Id), pagingParameters);
        }

        public async Task<PostCountViewModel> GetUserPostAndAppointmentCountAsync()
        {
            var myId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var sqlBuilder = new StringBuilder("select ");
            sqlBuilder.Append($"(select isnull(sum(case when PostType = 0 then 1 else 0 end), 0) from Posts where UserId = '{myId}') as PostCount,");
            sqlBuilder.Append($"(select isnull(sum(case when PostType = 1 then 1 else 0 end), 0) from Posts where UserId = '{myId}') as AppointmentCount,");
            sqlBuilder.Append($"(select isnull(sum(case when UserPostRelationType = 1 then 1 else 0 end), 0) from UserPostRelations where UserId = '{myId}') as LikedPostCount");

            return await _postContext.PostCounts.FromSqlRaw(sqlBuilder.ToString()).FirstOrDefaultAsync();
        }
    }
}
