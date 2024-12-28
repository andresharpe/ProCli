using ProCli.Cli.Common;
using ProCli.Cli.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace ProCli.Cli.Commands.Login;

public sealed class LoginCommand(IConsoleWriter console, IPersistedSecretCache tokenCache, AppSettings appSettings)
    : AsyncCommand<LoginCommand.Settings>
{
    private readonly IConsoleWriter _console = console;
    private readonly IPersistedSecretCache _tokenCache = tokenCache;
    private readonly AppSettings _appSettings = appSettings;

    public class Settings : LoggedInSettings
    {
        [CommandOption("-u|--username <USERNAME>")]
        [Description("Your user name.")]
        public string? Username { get; set; } = default!;

        [CommandOption("-p|--password <PASSWORD>")]
        [Description("Your password.")]
        public string? Password { get; set; } = default!;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var currentSettings = _appSettings;

        var usernamePrompt = new TextPrompt<string>($"[{Globals.StyleNormal.Foreground}]Enter your user name:[/]")
            .PromptStyle(Globals.StyleAlertAccent)
            .DefaultValueStyle(Globals.StyleDim)
            .DefaultValue(currentSettings?.Username ?? string.Empty)
            .Validate(ValidateUsername);

        var username = settings.Username ?? _console.Prompt(usernamePrompt);

        _console.WriteBlankLine();

        var passwordPrompt = new TextPrompt<string>($"[{Globals.StyleNormal.Foreground}]Enter your password:[/]")
            .PromptStyle(Globals.StyleAlertAccent)
            .DefaultValueStyle(Globals.StyleDim)
            .Secret()
            .DefaultValue(currentSettings?.Password ?? string.Empty)
            .Validate(ValidatePassword);

        var password = settings.Password ?? _console.Prompt(passwordPrompt);

        await _tokenCache.SaveAsync(Globals.AppName, new AppSettings()
        {
            Username = username,
            Password = password,
        });

        return 0;
    }

    private ValidationResult ValidateUsername(string value)
    {
        if (value.Length < 3) return ValidationResult.Error("Invalid user name.");

        return ValidationResult.Success();
    }

    private ValidationResult ValidatePassword(string value)
    {
        if (value.Length < 3) return ValidationResult.Error("Invalid user name.");

        return ValidationResult.Success();
    }
}