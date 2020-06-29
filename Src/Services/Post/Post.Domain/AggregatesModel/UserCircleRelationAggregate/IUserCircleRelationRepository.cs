using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.UserCircleRelationAggregate
{
    public interface IUserCircleRelationRepository : IRepository<UserCircleRelation>
    {
        /// <summary>
        /// 获取指定圈子与用户的关系列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        Task<List<UserCircleRelation>> GetRelationsByCircleIdAsync(Guid circleId);

        /// <summary>
        /// 获取指定圈子与指定用户的的关系
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserCircleRelation> GetRelationAsync(Guid circleId, Guid userId);

        /// <summary>
        /// 获取指定用户的置顶圈子关系
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserCircleRelation> GetToppingCircleRelationAsync(Guid userId);
    }
}
