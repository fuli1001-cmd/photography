using Arise.DDD.API.Paging;
using Photography.Services.Post.API.Query.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.API.Query.Interfaces
{
    public interface ICircleQueries
    {
        // 分页获取我的圈子
        Task<PagedList<CircleViewModel>> GetMyCirclesAsync(PagingParameters pagingParameters);

        // 分页获取所有的圈子
        Task<PagedList<CircleViewModel>> GetCirclesAsync(PagingParameters pagingParameters);

        Task<CircleViewModel> GetCircleAsync(Guid circleId);
    }
}
