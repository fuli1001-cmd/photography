using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetUsersAsync(List<Guid> userIds);

        /// <summary>
        /// 根据昵称数组获得用户id数组
        /// </summary>
        /// <param name="nicknames"></param>
        /// <returns></returns>
        Task<List<Guid>> GetUserIdsByNicknameAsync(List<string> nicknames);
    }
}
