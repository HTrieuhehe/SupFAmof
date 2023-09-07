using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Data.Redis
{
    public class Redis : IRedis
    {
        private readonly IRedisClientsManager _redisClientsManager;

        public Redis(IRedisClientsManager redisClientsManager)
        {
            _redisClientsManager = redisClientsManager;
        }

        public T Get<T>(string key)
        {
            using (var redis = _redisClientsManager.GetClient())
            {
                return redis.Get<T>(key);
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            using (var redis = _redisClientsManager.GetClient())
            {
                if (expiry.HasValue)
                {
                    redis.Set(key, value, expiry.Value);
                }
                else
                {
                    redis.Set(key, value);
                }
            }
        }
    }
}
