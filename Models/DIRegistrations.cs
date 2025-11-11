using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static partial class DIRegistrations
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services)
        {
            services.AddMemoryCache(); 
            services.AddScoped<ISubtopicRepository, SubtopicRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            return services;
        }

        // Register DAL with configuration (connection string, options)
        public static IServiceCollection AddDALServices(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(DALSettings));
            services.Configure<DALSettings>(section);

            var dalSettings = section.Get<DALSettings>()
                ?? throw new InvalidOperationException("DALSettings section missing in configuration.");

            // (command-line args or environment variables can override this)
            var defaultConnectionName = configuration["DAL:ConnectionStringName"] ?? dalSettings.RemoteConnectionString;
            services.AddSingleton<IConnectionStringProvider>(sp =>
                new ConfigConnectionStringProvider(sp.GetRequiredService<IConfiguration>(), defaultConnectionName));

            services.AddDbContextFactory();

            services.AddDALServices();

            return services;
        }

        private static void AddDbContextFactory(this IServiceCollection services)
        {
            services.AddDbContextFactory<ToDoContext>((sp, opts) =>
            {
                var connStrProvider = sp.GetRequiredService<IConnectionStringProvider>();
                var connectionString = connStrProvider.GetConnectionString();
                opts.UseSqlServer(connectionString, sqlOpts =>
                {
                    sqlOpts.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
            });
        }
    }
}
