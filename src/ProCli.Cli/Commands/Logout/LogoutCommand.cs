using ProCli.Cli.Commands.Login;
using ProCli.Cli.Common;
using ProCli.Cli.Configuration;
using Spectre.Console.Cli;

namespace ProCli.Cli.Commands.Logout;

public class LogoutCommand(IConsoleWriter console,
    IPersistedSecretCache tokenCache) : AsyncCommand<LoggedInSettings>
{
    private readonly IConsoleWriter _console = console;
    private readonly IPersistedSecretCache _tokenCache = tokenCache;

    public override Task<int> ExecuteAsync(CommandContext context, LoggedInSettings settings)
    {
        _tokenCache.Clear(Globals.AppName);

        _console.WriteNormal("You are logged out.");

        return Task.FromResult(0);
    }
}