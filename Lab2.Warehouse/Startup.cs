using Cassandra;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Lab2.Warehouse.Entities;
using Lab2.Warehouse.Filters.ExceptionFilters;
using Lab2.Warehouse.Repositories;
using Lab2.Warehouse.Repositories.Abstractions;

namespace Lab2.Warehouse {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            MappingConfiguration.Global.Define<IngredientMapping>();

            var cassandraConnectionString = Configuration.GetConnectionString("Cassandra");
            var cluster = Cluster.Builder()
                                 .WithConnectionString(cassandraConnectionString)
                                 .Build();
            var session = cluster.Connect("pad");
            services.TryAddSingleton<ISession>(session);
            services.TryAddScoped(typeof(IRepository<>), typeof(CassandraRepository<>));

            services.AddControllers(options => { options.Filters.Add<RequestExceptionFilter>(); })
                    .AddNewtonsoftJson(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
                    .AddXmlSerializerFormatters()
                    .AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lab2.Warehouse", Version = "v1" }); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab2.Warehouse v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
