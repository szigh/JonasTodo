public interface ISecretHelper
{
    Task<string?> GetSecretValueAsync(string secretName, CancellationToken cancellationToken = default);
    Task<string?> GetConnectionStringAsync(string connectionName, CancellationToken cancellationToken = default);
}
