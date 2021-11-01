using Lab2.SmartProxy.Cache;
using Lab2.SmartProxy.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lab2.SmartProxy {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var cacheConfig       = Configuration.GetSection("Cache").Get<CacheConfig>();
            var proxyCacheProfile = new CacheProfile { VaryByHeader = "Accept", Duration = cacheConfig.MaxAge };

            services.AddControllers()
                    .AddMvcOptions(options => options.CacheProfiles.Add("proxy", proxyCacheProfile));
            services.AddProxy(Configuration);
            services.AddDistributedCache(Configuration);
            services.AddDistributedResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseDistributedResponseCaching();
            app.UseEndpoints(endpoints => { endpoints.MapFallbackToController("{*path}", "Index", "Proxy"); });
        }
    }
}
