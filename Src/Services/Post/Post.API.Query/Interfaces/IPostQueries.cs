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
        Task<List<PostViewModel>> GetLikedPostsAsync();
        Task<List<PostViewModel>> GetHotPostsAsync();
        Task<List<PostViewModel>> GetFollowedPostsAsync();
        Task<List<PostViewModel>> GetSameCityPostsAsync(string cityCode);

        Task<PostViewModel> GetPostAsync(Guid postId);

        Task<List<PostViewModel>> SearchPosts(string key, string cityCode);
    }
}
