using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface ICommentQueries
    {
        Task<List<CommentViewModel>> GetPostCommentsAsync(Guid postId, int subCommentsCount);

        Task<List<CommentViewModel>> GetSubCommentsAsync(Guid commentId, int subCommentsCount);
    }
}
