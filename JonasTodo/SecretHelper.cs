using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace JonasTodoConsole
{
    public class SecretHelper : ISecretHelper
    {
        private readonly SecretClient _client;

        public SecretHelper(string vaultUri)
            : this(new SecretClient(
                new Uri(vaultUri ?? throw new ArgumentNullException(nameof(vaultUri))),
                new DefaultAzureCredential()))
        {
        }

        public SecretHelper(SecretClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string?> GetSecretValueAsync(string secretName, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(secretName)) 
                throw new ArgumentException("secretName must be provided", nameof(secretName));

            try
            {
                var response = await _client.GetSecretAsync(secretName, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                return response.Value?.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Secret not found
                return null;
            }
            // Let other exceptions bubble up (auth/permission/network)
        }

        // Convenience: maps "DefaultConnection" -> "ConnectionStrings--DefaultConnection"
        public Task<string?> GetConnectionStringAsync(string connectionName, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(connectionName)) 
                throw new ArgumentException("connectionName must be provided");
            var secretName = $"ConnectionStrings--{connectionName}";
            return GetSecretValueAsync(secretName, cancellationToken);
        }
    }
}