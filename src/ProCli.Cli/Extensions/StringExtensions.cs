namespace ProCli.Cli.Extensions;

using System.Text.RegularExpressions;

public static partial class StringExtensions
{
    public static string CamelToPascalCase(this string value)
    {
        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    public static string[] SplitCamelCase(this string input)
    {
        return UpperCaseRegex().Replace(input, ";$1").Trim().Split(';');
    }

    public static string? UnQuote(this string? input)
    {
        if (input == null) return null;
        if (input.Length >= 2 && input[0] == '"' && input[^1] == '"')
        {
            return input[1..^1]; // Using range syntax to slice the string.
        }
        return input;
    }

    public static string RemoveFromEnd(this string original, string toRemove)
    {
        if (original.EndsWith(toRemove))
        {
            return original[..^toRemove.Length];
        }
        return original;
    }

    public static string RemoveNewLines(this string text)
    {
        if (text.Contains('\n'))
        {
            text = text.Replace("\n", "");
        }

        if (text.Contains('\r'))
        {
            text = text.Replace("\r", "");
        }

        return text;
    }

    public static string Snip(this string text, int snipTo)
    {
        if (text.Length <= snipTo) return text;

        return text[..(snipTo - 1)] + "..";
    }

    [GeneratedRegex("([A-Z])", RegexOptions.Compiled)]
    private static partial Regex UpperCaseRegex();
}