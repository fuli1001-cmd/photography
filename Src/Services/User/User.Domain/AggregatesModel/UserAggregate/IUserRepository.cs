using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.User.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUserNameAsync(string userName);

        Task<Guid?> GetUserIdByCodeAsync(string code);
    }
}
