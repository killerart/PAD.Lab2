using System;
using System.Net.Http;
using Lab2.SmartProxy.Proxy.LoadBalancer;
using Lab2.SmartProxy.Proxy.LoadBalancer.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Lab2.SmartProxy.Proxy {
    public static class ProxyExtensions {
        public static IServiceCollection AddProxy(this IServiceCollection services, IConfiguration configuration) {
            var proxyConfig = configuration.GetSection("Proxy").Get<ProxyConfig>();
            services.AddSingleton(proxyConfig);
            foreach (var node in proxyConfig.Nodes) {
                services.AddHttpClient(node, client => client.BaseAddress = new Uri(node))
                        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
                        .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(1, TimeSpan.FromMinutes(1)));
            }

            services.AddSingleton<ILoadBalancer, RoundRobinLoadBalancer>();
            return services;
        }
    }
}
