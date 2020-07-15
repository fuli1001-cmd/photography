using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.UserShareAggregate
{
    public interface IUserShareRepository : IRepository<UserShare>
    {
        /// <summary>
        /// 根据分享者id和被分享者id获取UserShare
        /// </summary>
        /// <param name="sharerId">分享者id和被分享者id，两者一样</param>
        /// <returns></returns>
        Task<UserShare> GetUserShareAsync(Guid sharerId);

        /// <summary>
        /// 根据分享者和分享的帖子id获取UserShare
        /// </summary>
        /// <param name="sharerId">分享者id</param>
        /// <param name="postId">被分享的帖子id</param>
        /// <returns></returns>
        Task<UserShare> GetUserShareAsync(Guid sharerId, Guid postId);

        /// <summary>
        /// 根据分享者id和被分享的帖子类别获取UserShare
        /// </summary>
        /// <param name="sharerId">分享者id</param>
        /// <param name="privateTag">被分享的帖子类别</param>
        /// <returns></returns>
        Task<UserShare> GetUserShareAsync(Guid sharerId, string privateTag);
    }
}
