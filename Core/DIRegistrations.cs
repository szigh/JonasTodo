using DAL;
using JonasTodo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public static class DIRegistrations
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<ISelectTable, SelectTable>();
            return services;
        }
    }
}
