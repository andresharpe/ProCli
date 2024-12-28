using ProCli.Cli.Configuration;

namespace ProCli.Cli.Common;

public interface IPersistedSecretCache
{
    void Clear(string tokenName);

    Task<AppSettings?> LoadAsync(string tokenName);

    Task SaveAsync(string tokenName, AppSettings settings);
}