using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query
{
    public interface IPostQueries
    {
        IEnumerable<PostViewModel> GetPostsAsync(PostType postType);
    }
}
