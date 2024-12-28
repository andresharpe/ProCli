using ProCli.Cli.Common;
using Spectre.Console;

namespace ProCli.Cli.Configuration;

public static class Globals
{
    public const string AppName = "pro";
    public const string AppLongName = "Professional fully-featured cli template.";
    public const string AppDescription = "Cli template with multi-mode support, dependency injection, secure credential storage, basic commands.";
    public const string AppMoreInfo = "https://github.com/andresharpe/procli";
    public const string ProjectReleasePage = $"https://github.com/andresharpe/{Globals.AppName}/releases/latest";
    public const bool CheckProjectReleasePage = false;

    public static readonly string AppVersion = VersionChecker.GetInstalledCliVersion();

    public static readonly Style StyleHeading = new(Color.White, null, Decoration.Bold);
    public static readonly Style StyleSubHeading = new(Color.MistyRose3, null, Decoration.Bold);
    public static readonly Style StyleNormal = new(Color.LightSkyBlue3);
    public static readonly Style StyleDim = new(Color.LightPink4);
    public static readonly Style StyleAlert = new(Color.DarkOrange);
    public static readonly Style StyleAlertAccent = new(Color.Yellow4_1, null, Decoration.Bold);
    public static readonly Style StyleCode = new(Color.White, new Color(31, 22, 22));
    public static readonly Style StyleCodeHeading = new(Color.White, new Color(20, 14, 14));
    public static readonly Style StyleInput = new(new Color(192, 192, 192), new Color(40, 40, 40));
}