using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using JonasTodoConsole;
using Microsoft.Extensions.DependencyInjection;

public static class SecretHelperServiceCollectionExtensions
{
    // Register SecretClient + SecretHelper
    public static IServiceCollection AddKeyVaultSecretHelper(this IServiceCollection services, 
        string vaultUri)
    {
        if (string.IsNullOrWhiteSpace(vaultUri)) 
            throw new ArgumentException("vaultUri must be provided", nameof(vaultUri));

        var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
        services.AddSingleton(client);
        services.AddSingleton<ISecretHelper>(sp => 
            new SecretHelper(sp.GetRequiredService<SecretClient>()));
        return services;
    }
}
