using Arise.DDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Photography.Services.Post.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        //Task<Guid?> GetUserIdByCodeAsync(string code);
    }
}
