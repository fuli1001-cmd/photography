using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IPostQueries
    {
        /// <summary>
        /// 用户的帖子
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="privateTag">帖子类别</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="pagingParameters">分页参数</param>
        /// <returns></returns>
        Task<PagedList<PostViewModel>> GetUserPostsAsync(Guid userId, string privateTag, string key, PagingParameters pagingParameters);

        // 赞过的帖子
        Task<PagedList<PostViewModel>> GetLikedPostsAsync(PagingParameters pagingParameters);

        // 热门帖子
        Task<PagedList<PostViewModel>> GetHotPostsAsync(PagingParameters pagingParameters);

        // 关注的人的帖子
        Task<PagedList<PostViewModel>> GetFollowedPostsAsync(PagingParameters pagingParameters);

        // 同城的帖子
        Task<PagedList<PostViewModel>> GetSameCityPostsAsync(string cityCode, PagingParameters pagingParameters);

        // 获取系统标签下的帖子
        Task<PagedList<PostViewModel>> GetPostsBySystemTag(string systemTag, PagingParameters pagingParameters);

        // 获取指定id列表的帖子
        Task<List<PostViewModel>> GetPostsAsync(List<Guid> postIds);

        // 获取帖子详情
        Task<PostViewModel> GetPostAsync(Guid postId);

        // 获取分享的帖子
        Task<PostViewModel> GetSharedPostAsync(Guid postId, Guid sharedUserId);

        // 分页获取分享的类别下的所有帖子
        Task<PagedList<PostViewModel>> GetSharedPostsAsync(string privateTag, Guid sharedUserId, string key, PagingParameters pagingParameters);

        // 分页获取分享的用户的所有帖子
        Task<PagedList<PostViewModel>> GetSharedPostsAsync(Guid sharedUserId, string key, PagingParameters pagingParameters);

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

        Task<PagedList<Domain.AggregatesModel.PostAggregate.Post>> GetPostsAsync(PagingParameters pagingParameters);

        /// <summary>
        /// 获取用户的3种帖子数量：帖子数量，约拍数量，点赞的帖子数量
        /// </summary>
        /// <returns></returns>
        Task<PostCountViewModel> GetUserPostAndAppointmentCountAsync();

        /// <summary>
        /// 根据帖子的可见性返回帖子中被@的用户id，包括其转发的帖子中@的用户id
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        Task<List<Guid>> GetAtUserIdsAsync(Domain.AggregatesModel.PostAggregate.Post post);
    }
}
