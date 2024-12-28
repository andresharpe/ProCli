using ProCli.Cli.Common;
using ProCli.Cli.Extensions;
using System.Runtime.Serialization;

namespace ProCli.Cli.Configuration;

public class AppSettings
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string WeatherApiKey { get; set; } = default!;
    public string OpenTelemetryEndpoint { get; set; } = default!;
    public string OpenTelemetryApiKey { get; set; } = default!;

    [OnDeserialized]
    internal void GetFromEnvironment(StreamingContext context)
    {
        GetFromEnvironment();
    }

    internal AppSettings GetFromEnvironment()
    {
        var prefix = $"{Globals.AppName.CamelToPascalCase()}__";

        var envValues = EnvironmentVars.GetAll().Where(kv => kv.Key.StartsWith(prefix)).ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var prop in typeof(AppSettings).GetProperties())
        {
            if (envValues.TryGetValue($"{prefix}{prop.Name}", out var value))
            {
                prop.SetValue(this, value);
            }
        }
        return this;
    }

    public IReadOnlyDictionary<string, string?> GetSettings()
    {
        var prefix = $"{Globals.AppName.CamelToPascalCase()}__";

        var envValues = EnvironmentVars.GetAll().Where(kv => kv.Key.StartsWith(prefix));

        Dictionary<string, string?> returnVal = [];

        foreach (var prop in typeof(AppSettings).GetProperties())
        {
            returnVal[$"{prefix}{prop.Name}"] = prop.GetValue(this)?.ToString();
        }

        foreach (var kv in envValues)
        {
            returnVal[kv.Key] = kv.Value;
        }

        return returnVal;
    }
}