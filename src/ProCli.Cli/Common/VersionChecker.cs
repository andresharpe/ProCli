#pragma warning disable CS0162 // Unreachable code detected

using ProCli.Cli.Configuration;
using Spectre.Console;
using System.Reflection;

namespace ProCli.Cli.Common;

public static class VersionChecker
{
    public static async Task CheckForLatestVersion()
    {
        try
        {
            var installedVersion = GetInstalledCliVersion();

            var latestVersion = installedVersion;

            await Task.Yield();

            if (Globals.CheckProjectReleasePage)
            {
                latestVersion = await GetProjectReleaseVersion();
            }

            var installedVersionNo = Convert.ToInt32(installedVersion.Replace(".", ""));

            var latestVersionNo = Convert.ToInt32(latestVersion.Replace(".", ""));

            if (installedVersionNo < latestVersionNo && installedVersionNo > 100)
            {
                var cw = new ConsoleWriter(AnsiConsole.Console);

                cw.WriteDim();
                cw.WriteBlankLine();
                cw.WriteAlert($"This version of '{Globals.AppName}' ({installedVersion}) is older than that of the latest version ({latestVersion}). Update the tool for the latest features and bug fixes:");
                cw.WriteBlankLine();
                cw.WriteAlertAccent($"dotnet tool update -g {Globals.AppName}");
                cw.WriteBlankLine();
            }
        }
        catch (Exception)
        {
            // fail silently
        }
    }

    private static async Task<string?> GetProjectReleaseVersion()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, Globals.ProjectReleasePage);

        request.Headers.Add("Accept", "text/html");

        using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

        using var response = await client.SendAsync(request);

        using var content = response.Content;

        if (response.StatusCode != System.Net.HttpStatusCode.Found)
        {
            return null;
        }

        if (response.Headers is null)
        {
            return null;
        }

        if (response.Headers.Location is null)
        {
            return null;
        }

        var latestVersion = response.Headers.Location.Segments.LastOrDefault();

        if (latestVersion is null)
        {
            return null;
        }

        if (latestVersion.FirstOrDefault() == 'v')
        {
            latestVersion = latestVersion[1..]; // remove the 'v' prefix. equivalent to `latest.Substring(1, latest.Length - 1)`
        }

        return latestVersion;
    }

    public static string GetInstalledCliVersion()
    {
        var installedVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        installedVersion = installedVersion[0..^2];

        return installedVersion;
    }
}