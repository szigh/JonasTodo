using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DAL.Models;

namespace DAL
{
    public static class DIRegistrations
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services)
        {
            // Register DAL-specific types (repositories, caches, etc.) here, e.g.:
            // services.AddScoped<IMyRepository, MyRepository>();
            // (None are present in the current codebase so this is intentionally empty.)
            return services;
        }

        // Register DAL with configuration (connection string, options)
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DALSettings>(configuration.GetSection(nameof(DALSettings)));

            var conn = configuration.GetConnectionString("ToDoApp");
            services.AddDbContextFactory<ToDoContext>(opts => opts.UseSqlServer(conn));

            services.AddDALServices();

            return services;
        }
    }
}
