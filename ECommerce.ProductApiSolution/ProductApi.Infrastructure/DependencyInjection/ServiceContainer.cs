using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDBContext>(services, config, config["MySerilog:FineName"]!);

            // Create Dependency Injection
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register Middleware such as:
            // GlobalException: handles external errors.
            // Listen to only Api gatewy: blocks all outside calls;
            SharedServiceContainer.UserSharedPolicies(app);

            return app;
        }
    }
}
