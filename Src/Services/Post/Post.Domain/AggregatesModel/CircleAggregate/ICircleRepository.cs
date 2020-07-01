using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.CircleAggregate
{
    public interface ICircleRepository : IRepository<Circle>
    {
        Task<Circle> GetCircleByNameAsync(string name);

        Task<Circle> GetCircleWithPostsAsync(Guid circleId);

        /// <summary>
        /// 用户的圈子数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> GetUserCircleCount(Guid userId);
    }
}
