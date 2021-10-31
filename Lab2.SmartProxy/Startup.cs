using System.Net.Http;
using Lab2.SmartProxy.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Lab2.SmartProxy {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddProxy(Configuration);

            var redisConfig = Configuration.GetSection("Redis");
            if (redisConfig.Exists())
                services.AddStackExchangeRedisCache(options => {
                    //options.Configuration = redisConfig["ConnectionString"];
                    options.InstanceName                  = redisConfig["InstanceName"];
                    options.ConfigurationOptions          = ConfigurationOptions.Parse(redisConfig["ConnectionString"]);
                    options.ConfigurationOptions.Password = redisConfig["Password"];
                });
            else
                services.AddDistributedMemoryCache();

            services.AddDistributedResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseDistributedResponseCaching();

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapFallbackToController("{*path}", "Index", "Proxy");
            });
        }
    }
}
