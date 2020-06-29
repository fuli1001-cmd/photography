using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IPostQueries
    {
        // 用户的帖子
        Task<PagedList<PostViewModel>> GetUserPostsAsync(Guid userId, string privateTag, string key, PagingParameters pagingParameters);

        // 赞过的帖子
        Task<PagedList<PostViewModel>> GetLikedPostsAsync(PagingParameters pagingParameters);

        // 热门帖子
        Task<PagedList<PostViewModel>> GetHotPostsAsync(PagingParameters pagingParameters);

        // 关注的人的帖子
        Task<PagedList<PostViewModel>> GetFollowedPostsAsync(PagingParameters pagingParameters);

        // 同城的帖子
        Task<PagedList<PostViewModel>> GetSameCityPostsAsync(string cityCode, PagingParameters pagingParameters);

        // 获取指定id列表的帖子
        Task<List<PostViewModel>> GetPostsAsync(List<Guid> postIds);

        // 获取帖子详情
        Task<PostViewModel> GetPostAsync(Guid postId);

        // 获取分享的帖子的详情
        Task<PostViewModel> GetSharedPostAsync(Guid postId, Guid sharedUserId);

        // 搜索帖子
        Task<PagedList<PostViewModel>> SearchPosts(string key, string cityCode, PagingParameters pagingParameters);

        // 按公有标签获取帖子
        Task<PagedList<PostViewModel>> GetPostsByPublicTagAsync(string tag, PagingParameters pagingParameters);

        /// <summary>
        /// 指定圈子的帖子
        /// </summary>
        /// <param name="circleId">圈子id</param>
        /// <param name="onlyGood">只查询精华帖</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="sortBy">排序字段，score：按帖子分数排序，其它：按更新时间排序</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        Task<PagedList<PostViewModel>> GetCirclePostsAsync(Guid circleId, bool onlyGood, string key, string sortBy, PagingParameters pagingParameters);
    }
}
