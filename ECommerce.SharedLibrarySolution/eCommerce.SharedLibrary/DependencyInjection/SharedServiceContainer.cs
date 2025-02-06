using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            // Add Generic Database context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"), sqlseverOption =>
                sqlseverOption.EnableRetryOnFailure()));

            // configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp: yyyy-MM-dd HH:mm:ss.fff zzz} [Level: u3}] {message:lj}{NewLine}{Exceptions}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Add JWT authentication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
            return services;
        }

        public static IApplicationBuilder UserSharedPolicies(this IApplicationBuilder app)
        {
            // Use global Exception
            app.UseMiddleware<GlobalException>();

            // Register middlerware to block all outsiders API calls
            //app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
