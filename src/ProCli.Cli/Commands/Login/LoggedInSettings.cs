using ProCli.Cli.Commands.BaseCommands;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace ProCli.Cli.Commands.Login;

public class LoggedInSettings : CommonSettings
{
    [CommandOption("--force")]
    [Description("Forces warning and validation prompts to be bypassed.")]
    public bool Force { get; set; }
}