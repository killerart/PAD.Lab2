using Lab2.Shared.ServiceInstaller;
using Lab2.SmartProxy.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.SmartProxy.ServiceInstallers {
    public class MvcInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
            var cacheConfig       = configuration.GetSection("Cache").Get<CacheConfig>();
            var proxyCacheProfile = new CacheProfile { VaryByHeader = "Accept", Duration = cacheConfig.MaxAge };

            services.AddControllers()
                    .AddMvcOptions(options => options.CacheProfiles.Add("proxy", proxyCacheProfile));
        }
    }
}
