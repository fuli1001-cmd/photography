using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Services.User.API.Settings;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photography.Services.User.API.Infrastructure.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IOptionsSnapshot<RedisSettings> _redisSettings;
        private readonly ILogger<RedisService> _logger;
        private ConnectionMultiplexer _redis;

        public RedisService(IOptionsSnapshot<RedisSettings> redisSettings, ILogger<RedisService> logger)
        {
            _redisSettings = redisSettings ?? throw new ArgumentNullException(nameof(redisSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Connect();
        }

        public async Task<string> Get(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task Set(string key, string value)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value);
        }

        private void Connect()
        {
            try
            {
                var configString = $"{_redisSettings.Value.Host}:{_redisSettings.Value.Port},connectRetry=5";
                _redis = ConnectionMultiplexer.Connect(configString);
            }
            catch (RedisConnectionException err)
            {
                _logger.LogError(err.ToString());
                throw err;
            }
        }
    }
}
