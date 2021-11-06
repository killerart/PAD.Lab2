using Cassandra.Mapping;
using Lab2.Shared.ServiceInstaller;
using Lab2.Warehouse.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Warehouse.ServiceInstallers {
    public class MapsterInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
            MappingConfiguration.Global.Define<IngredientMapping>();
        }
    }
}
