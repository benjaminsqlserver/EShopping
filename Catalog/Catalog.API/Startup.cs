using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Catalog.API
{
    public class Startup
    {
        private  IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddApiVersioning();
            services.AddHealthChecks()
           .AddMongoDb(Configuration["DatabaseSettings:ConnectionString"], "Catalog  Mongo Db Health Check",
               HealthStatus.Degraded);
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" }); });


            //DI
            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(CreateProductHandler).GetTypeInfo().Assembly);
           
            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, ProductRepository>();
            services.AddScoped<ITypesRepository, ProductRepository>();
            services.AddControllers();
            
        }

        public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));

            }
            app.UseRouting();
            app.UseStaticFiles();
         
           
            app.UseAuthorization();
            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            }
            );

         
        }
    }
}
