using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface ICommentQueries
    {
        Task<PagedList<CommentViewModel>> GetPostCommentsAsync(Guid postId, int subCommentsCount, PagingParameters pagingParameters);

        Task<PagedList<CommentViewModel>> GetSubCommentsAsync(Guid commentId, int subCommentsCount, PagingParameters pagingParameters);
    }
}
