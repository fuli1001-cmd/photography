using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task Set(string key, string value);
        Task<string> Get(string key);
    }
}
