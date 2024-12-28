using Microsoft.AspNetCore.DataProtection;
using ProCli.Cli.Configuration;
using ProCli.Cli.Exceptions;
using System.Text.Json;

namespace ProCli.Cli.Common;

public class PersistedSecretCache(IDataProtectionProvider provider) : IPersistedSecretCache
{
    private readonly IDataProtectionProvider _provider = provider;

    private const string _protectorPurpose = $"{Globals.AppName}-settings";

    public Task SaveAsync(string tokenName, AppSettings settings)
    {
        var protector = _provider.CreateProtector(_protectorPurpose);
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".{tokenName}");
        var content = JsonSerializer.Serialize(settings);
        return File.WriteAllTextAsync(path, protector.Protect(content));
    }

    public async Task<AppSettings?> LoadAsync(string tokenName)
    {
        var protector = _provider.CreateProtector(_protectorPurpose);
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".{tokenName}");
        if (!File.Exists(path)) return TryLoadFromEnvironment();
        var content = await File.ReadAllTextAsync(path);
        try
        {
            var json = protector.Unprotect(content);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }
        catch
        {
            throw new CliException($"The secure store may be corrupt. ({path})");
        }
    }

    public void Clear(string tokenName)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".{tokenName}");

        if (!File.Exists(path)) return;

        File.Delete(path);
    }

    private static AppSettings? TryLoadFromEnvironment()
    {
        var settings = new AppSettings().GetFromEnvironment();

        return settings;
    }
}