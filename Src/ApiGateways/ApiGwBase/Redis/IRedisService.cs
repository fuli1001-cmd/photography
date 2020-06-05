using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.ApiGateways.ApiGwBase.Redis
{
    public interface IRedisService
    {
        Task SetAsync(string key, RedisValue value, TimeSpan? expireTimeSpan);
        Task<string> GetAsync(string key);
    }
}
