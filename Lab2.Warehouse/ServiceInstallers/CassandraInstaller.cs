using Cassandra;
using Lab2.Shared.ServiceInstaller;
using Lab2.Warehouse.Repositories;
using Lab2.Warehouse.Repositories.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lab2.Warehouse.ServiceInstallers {
    public class CassandraInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
            var cassandraConnectionString = configuration.GetConnectionString("Cassandra");
            var cluster = Cluster.Builder()
                                 .WithConnectionString(cassandraConnectionString)
                                 .Build();
            var session = cluster.Connect("pad");
            services.TryAddSingleton<ISession>(session);
            services.TryAddScoped(typeof(IRepository<>), typeof(CassandraRepository<>));
        }
    }
}
