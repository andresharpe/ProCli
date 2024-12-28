using Microsoft.AspNetCore.DataProtection;
using ProCli.Cli.Commands.Login;
using ProCli.Cli.Commands.Logout;
using ProCli.Cli.Commands.Version;
using ProCli.Cli.Common;
using ProCli.Cli.Configuration;
using ProCli.Cli.Exceptions;
using Serilog;
using Serilog.Events;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

namespace ProCli.Cli;

public class CommandAppBuilder
{
    private readonly bool _isGettingVersion;
    private readonly bool _showBanner;
    private readonly bool _showLogOutput;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly AppSettings _appSettings;
    private readonly ServiceCollection _services;

    public bool IsGettingVersion => _isGettingVersion;
    public bool ShowBanner => _showBanner;
    public bool ShowLogOutput => _showLogOutput;

    public CommandAppBuilder(string[] args)
    {
        _isGettingVersion = args.Contains("version");

        _showBanner = !_isGettingVersion && !args.Contains("--no-banner") && !args.Contains("-x");

        _showLogOutput = args.Contains("--log-output");

        _dataProtectionProvider = DataProtectionProvider.Create(Globals.AppName);

        _appSettings = LoadAppSettings();

        ConfigureLogger();

        _services = BuildServiceCollection();
    }

    public CommandApp Build(Action<IConfigurator>? configurator = null)
    {
        var typeRegistrar = new TypeRegistrar(_services);

        var commandApp = new CommandApp(typeRegistrar);

        typeRegistrar.RegisterInstance(typeof(ICommandApp), commandApp);

        commandApp.Configure(commandConfig =>
        {
            commandConfig.SetApplicationName(Globals.AppName);

            commandConfig.PropagateExceptions();

            commandConfig.Settings.HelpProviderStyles = GetHelpProviderstyle();

            commandConfig.AddCommand<LoginCommand>("login")
                .WithDescription("Run this command first to login to the CLI.");

            commandConfig.AddCommand<LogoutCommand>("logout")
                .WithDescription("Log out of the CLI and clear all credentials from the current device.");
            /*
            commandConfig.AddBranch("weather", branchConfig =>
            {
                branchConfig.SetDescription("Configure multiple profiles (space, environment, tokens) for connecting to contentful.");

                branchConfig.AddCommand<FakeWeatherCommand>("fake")
                    .WithDescription("List all profiles.");

                branchConfig.AddCommand<RealWeatherCommand>("real")
                    .WithDescription("Add a new profile.");
            });

            */

            commandConfig.AddCommand<VersionCommand>("version")
                .WithDescription("Display the current installed version of the CLI.");

            configurator?.Invoke(commandConfig);
        });

        return commandApp;
    }

    private ServiceCollection BuildServiceCollection()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConsoleWriter, ConsoleWriter>();
        services.AddSingleton<IPersistedSecretCache, PersistedSecretCache>();
        services.AddSingleton(_dataProtectionProvider);
        services.AddSingleton(_appSettings);
        services.AddSingleton(_appSettings.GetSettings());
        services.AddHttpClient();
        services.AddLogging(builder => builder.ClearProviders().AddSerilog());

        return services;
    }

    private AppSettings LoadAppSettings()
    {
        var appSettings = new PersistedSecretCache(_dataProtectionProvider).LoadAsync(Globals.AppName).Result;

        return appSettings ??= new();
    }

    private void ConfigureLogger()
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext();

        if (_appSettings.OpenTelemetryEndpoint is not null && _appSettings.OpenTelemetryApiKey is not null)
        {
            loggerConfig.WriteTo.OpenTelemetry(o =>
            {
                o.Endpoint = _appSettings.OpenTelemetryEndpoint;
                o.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf;
                o.Headers = new Dictionary<string, string> { ["X-Seq-ApiKey"] = _appSettings.OpenTelemetryApiKey };
                o.ResourceAttributes = new Dictionary<string, object> { ["service.name"] = Globals.AppName };
                o.RestrictedToMinimumLevel = LogEventLevel.Information;
            });
        }

        if (_showLogOutput)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information
            );

            ConsoleWriter.EnableConsole = false;
        }

        Log.Logger = loggerConfig.CreateLogger();

        return;
    }

    public void WriteException(Exception ex)
    {
        var console = new ConsoleWriter(AnsiConsole.Console);

        if (ex is ICliException
            || ex is CommandParseException
            || ex is CommandRuntimeException
            || ex is HttpRequestException
            || ex is IOException
            )
        {
            var message = ex.InnerException?.Message ?? ex.Message;

            console.WriteLine($"Error: {message}", Globals.StyleAlert);
        }
        else
        {
            console.WriteException(ex);
        }
    }

    private static HelpProviderStyle GetHelpProviderstyle()
    {
        return new()
        {
            Arguments = new()
            {
                Header = Globals.StyleSubHeading,
                OptionalArgument = Globals.StyleDim,
                RequiredArgument = Globals.StyleAlertAccent,
            },

            Commands = new()
            {
                Header = Globals.StyleSubHeading,
                ChildCommand = Globals.StyleAlertAccent,
                RequiredArgument = Globals.StyleDim,
            },

            Options = new()
            {
                Header = Globals.StyleSubHeading,
                RequiredOption = Globals.StyleAlert,
                OptionalOption = Globals.StyleAlertAccent,
                DefaultValue = Globals.StyleDim,
                DefaultValueHeader = Globals.StyleNormal,
            },

            Description = new()
            {
                Header = Globals.StyleDim,
            },

            Usage = new()
            {
                Header = Globals.StyleSubHeading,
                Command = Globals.StyleAlertAccent,
                CurrentCommand = Globals.StyleAlert,
                Options = Globals.StyleDim,
            },

            Examples = new()
            {
                Header = Globals.StyleHeading,
                Arguments = Globals.StyleAlertAccent,
            }
        };
    }
}