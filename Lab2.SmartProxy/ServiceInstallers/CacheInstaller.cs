using Lab2.Shared.ServiceInstaller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Lab2.SmartProxy.ServiceInstallers {
    public class CacheInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
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

            services.AddDistributedResponseCaching();
        }
    }
}
