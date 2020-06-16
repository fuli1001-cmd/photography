using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IPostQueries
    {
        Task<PagedList<PostViewModel>> GetUserPostsAsync(Guid userId, PagingParameters pagingParameters);
        Task<PagedList<PostViewModel>> GetLikedPostsAsync(PagingParameters pagingParameters);
        Task<PagedList<PostViewModel>> GetHotPostsAsync(PagingParameters pagingParameters);
        Task<PagedList<PostViewModel>> GetFollowedPostsAsync(PagingParameters pagingParameters);
        Task<PagedList<PostViewModel>> GetSameCityPostsAsync(string cityCode, PagingParameters pagingParameters);

        Task<List<PostViewModel>> GetPostsAsync(List<Guid> postIds);

        Task<PostViewModel> GetPostAsync(Guid postId);

        Task<PostViewModel> GetSharedPostAsync(Guid postId, Guid sharedUserId);

        Task<PagedList<PostViewModel>> SearchPosts(string key, string cityCode, PagingParameters pagingParameters);
    }
}
