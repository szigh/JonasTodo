//using DAL.Caches;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DIRegistrations
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services)
        {
            services.AddMemoryCache(); 
            services.AddScoped<ISubtopicRepository, SubtopicRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            //services.AddSingleton<ISubtopicCache, SubtopicCache>();
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
