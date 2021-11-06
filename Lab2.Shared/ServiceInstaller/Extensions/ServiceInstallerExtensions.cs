using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Shared.ServiceInstaller.Extensions {
    public static class ServiceInstallerExtensions {
        public static IServiceCollection AddInstallersFromAssemblyContaining<TMarker>(this IServiceCollection services, IConfiguration configuration) {
            return services.AddInstallersFromAssembliesContaining(configuration, typeof(TMarker));
        }

        public static IServiceCollection AddInstallersFromAssembliesContaining(this IServiceCollection services,
                                                                               IConfiguration          configuration,
                                                                               params Type[]           assemblyMarkers) {
            var assemblies = assemblyMarkers.Select(m => m.Assembly).ToArray();
            return services.AddInstallersFromAssemblies(configuration, assemblies);
        }

        public static IServiceCollection AddInstallersFromAssemblies(this IServiceCollection services,
                                                                     IConfiguration          configuration,
                                                                     params Assembly[]       assemblies) {
            foreach (var assembly in assemblies) {
                var installers = assembly.DefinedTypes
                                         .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IServiceInstaller).IsAssignableFrom(t))
                                         .Select(Activator.CreateInstance)
                                         .Cast<IServiceInstaller>();

                foreach (var installer in installers) {
                    installer.AddServices(services, configuration);
                }
            }

            return services;
        }
    }
}
