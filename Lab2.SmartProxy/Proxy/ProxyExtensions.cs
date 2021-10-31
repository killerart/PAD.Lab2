using System.Net.Http;
using Lab2.SmartProxy.Proxy.LoadBalancer;
using Lab2.SmartProxy.Proxy.LoadBalancer.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.SmartProxy.Proxy {
    public static class ProxyExtensions {
        public static IServiceCollection AddProxy(this IServiceCollection services, IConfiguration configuration) {
            services.AddHttpClient("proxy")
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
            services.AddSingleton(configuration.GetSection("Proxy").Get<ProxyConfig>());
            services.AddSingleton<ILoadBalancer, RoundRobinLoadBalancer>();
            // return services.AddSingleton<ProxyMiddleware>();
            return services;
        }

        // public static IApplicationBuilder UseProxy(this IApplicationBuilder app) {
        //     return app.UseMiddleware<ProxyMiddleware>();
        // }
    }
}
