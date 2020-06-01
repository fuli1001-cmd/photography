using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface IPostQueries
    {
        Task<List<PostViewModel>> GetUserPostsAsync(Guid userId);
        Task<List<PostViewModel>> GetLikedPostsAsync();
        Task<List<PostViewModel>> GetHotPostsAsync();
        Task<List<PostViewModel>> GetFollowedPostsAsync();
        Task<List<PostViewModel>> GetSameCityPostsAsync(string cityCode);

        Task<PostViewModel> GetPostAsync(Guid postId);
    }
}
