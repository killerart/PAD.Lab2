using Cassandra;
using Lab2.Shared.ServiceInstaller;
using Lab2.Warehouse.Repositories;
using Lab2.Warehouse.Repositories.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Warehouse.ServiceInstallers {
    public class CassandraInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
            var cassandraConnectionString = configuration.GetConnectionString("Cassandra");
            var cluster = Cluster.Builder()
                                 .WithConnectionString(cassandraConnectionString)
                                 .Build();
            var session = cluster.Connect("pad");
            services.AddSingleton<ISession>(session);
            services.AddScoped(typeof(IRepository<>), typeof(CassandraRepository<>));
        }
    }
}
