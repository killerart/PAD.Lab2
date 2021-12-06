using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Cassandra;
using Cassandra.Mapping;
using Lab2.Shared.ServiceInstaller;
using Lab2.Warehouse.Core;
using Lab2.Warehouse.Infrastructure.Mappings;
using Lab2.Warehouse.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Warehouse.ServiceInstallers {
    public class CassandraInstaller : IServiceInstaller {
        public void AddServices(IServiceCollection services, IConfiguration configuration) {
            MappingConfiguration.Global.Define<IngredientMapping>();

            var options = new SSLOptions(SslProtocols.Tls12, true, ValidateServerCertificate);
            options.SetHostNameResolver(_ => "pad.cassandra.cosmos.azure.com");

            var cassandraConnectionString = configuration.GetConnectionString("Cassandra");
            var cluster = Cluster.Builder()
                                 .WithConnectionString(cassandraConnectionString)
                                 .WithSSL(options)
                                 .Build();
            var session = cluster.Connect();
            services.AddSingleton<ISession>(session);
            services.AddScoped(typeof(IRepository<>), typeof(CassandraRepository<>));
        }

        public static bool ValidateServerCertificate(object           sender,
                                                     X509Certificate? certificate,
                                                     X509Chain?       chain,
                                                     SslPolicyErrors  sslPolicyErrors) {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }
    }
}
