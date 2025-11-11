using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public static partial class DIRegistrations
    {
        internal class ConfigConnectionStringProvider : IConnectionStringProvider
        {
            private readonly IConfiguration _configuration;
            private readonly string _defaultName;
            private string? _selectedName;

            public ConfigConnectionStringProvider(IConfiguration configuration, string defaultName)
            {
                _configuration = configuration;
                _defaultName = defaultName;
            }

            public string GetConnectionString()
            {
                // Check runtime override first (allows env var or command-line to change the selection)
                var key = _configuration["DAL:ConnectionStringName"] ?? _defaultName;

                //accesses Azure Key Vault to get the actual connection string
                var connectionString = _configuration.GetConnectionString(key);
                                
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException($"Connection string '{key}' not found.");

                return connectionString;
            }

            public void SetConnectionStringKey(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Name cannot be null or whitespace.", nameof(key));

                Interlocked.Exchange(ref _selectedName, key);
            }
        }
    }
}
