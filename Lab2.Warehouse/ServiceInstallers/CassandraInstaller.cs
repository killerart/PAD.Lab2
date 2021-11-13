using Cassandra;
using Cassandra.Mapping;
using Lab2.Shared.ServiceInstaller;
using Lab2.Warehouse.Core;
using Lab2.Warehouse.Domain.Entities;
using Lab2.Warehouse.Infrastructure.Mappings;
using Lab2.Warehouse.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Warehouse.ServiceInstallers {
    public class CassandraInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
            MappingConfiguration.Global.Define<IngredientMapping>();
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
