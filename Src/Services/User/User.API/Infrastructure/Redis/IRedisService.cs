using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expireTimeSpan);

        Task<string> StringGetAsync(RedisKey key);

        Task HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value);

        Task PublishAsync(RedisChannel channel, RedisValue value);
    }
}
