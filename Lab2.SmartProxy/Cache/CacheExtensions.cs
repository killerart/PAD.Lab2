using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Lab2.SmartProxy.Cache {
    public static class CacheExtensions {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration) {
            var redisConfig = configuration.GetSection("Redis");
            if (redisConfig.Exists()) {
                services.AddStackExchangeRedisCache(options => {
                    options.InstanceName                  = redisConfig["InstanceName"];
                    options.ConfigurationOptions          = ConfigurationOptions.Parse(redisConfig["ConnectionString"]);
                    options.ConfigurationOptions.Password = redisConfig["Password"];
                });
            } else {
                services.AddDistributedMemoryCache();
            }

            return services;
        }
    }
}
