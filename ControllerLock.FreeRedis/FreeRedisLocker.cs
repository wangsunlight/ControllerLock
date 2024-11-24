using ControllerLock.Abstractions;
using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FreeRedis.RedisClient;

namespace ControllerLock.FreeRedis
{
    public class FreeRedisLocker : ILocker
    {
        private readonly IRedisClient _redisClient;

        private RedisClient.LockController lockController;

        public FreeRedisLocker(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }
        public bool Lock(string key, string value, TimeSpan timeout, bool isdelay)
        {
            Console.WriteLine($"上锁key:{key}value{value}");
            if (isdelay)
            {
                lockController = _redisClient.Lock(key, timeout.Seconds, false);
                return true;
            }

            return _redisClient.SetNx(key, value, timeout);
        }

        public void UnLock(string key, string value, bool isdelay)
        {
            Console.WriteLine($"解锁key:{key}value{value}");
            if (isdelay)
            {
                lockController?.Unlock();
                return;
            }

            _redisClient.Del(key);
        }
    }
}
