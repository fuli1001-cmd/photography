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
        Task<PagedList<PostViewModel>> GetUserPostsAsync(Guid userId, PagingParameters pagingParameters);

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
    }
}
