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
        /// <summary>
        /// 分页获取我的圈子
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        Task<PagedList<CircleViewModel>> GetMyCirclesAsync(PagingParameters pagingParameters);

        /// <summary>
        /// 分页获取所有的圈子
        /// </summary>
        /// <param name="pagingParameters"></param>
        /// <returns></returns>
        Task<PagedList<CircleViewModel>> GetCirclesAsync(PagingParameters pagingParameters);

        /// <summary>
        /// 圈子详情
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        Task<CircleViewModel> GetCircleAsync(Guid circleId);
    }
}
