using ControllerLock.FreeRedis;
using ControllerLock.Abstractions;
using FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerLock
{
    public static class FreeRedisServiceCollectionExtensions
    {
        public static IServiceCollection AddFreeRedisControllerLock(this IServiceCollection services, string connectionStr)
        {
            services.AddSingleton<IRedisClient>(new RedisClient(connectionStr));
            services.RemoveAll<ILocker>();
            services.RemoveAll<ILockerResult>();
            services.AddScoped<ILocker, FreeRedisLocker>();
            services.AddScoped<ILockerResult, FreeRedisResult>();

            return services;
        }
    }
}
