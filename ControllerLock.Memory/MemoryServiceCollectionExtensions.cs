using ControllerLock.Abstractions;
using ControllerLock.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ControllerLock
{
    public static class MemoryServiceCollectionExtensions
    {
        public static IServiceCollection AddMemoryControllerLock(this IServiceCollection services)
        {
            services.AddSingleton<LockProvider<string>>(new LockProvider<string>());
            services.RemoveAll<ILocker>();
            services.RemoveAll<ILockerResult>();
            services.AddScoped<ILocker, MemoryLocker>();
            services.AddScoped<ILockerResult, MemoryResult>();

            return services;
        }
    }
}
