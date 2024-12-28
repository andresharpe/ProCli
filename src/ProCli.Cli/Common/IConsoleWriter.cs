using Spectre.Console;
using Spectre.Console.Rendering;

namespace ProCli.Cli.Common;

public interface IConsoleWriter
{
    ILogger? Logger { get; set; }

    string FormatToMarkup(FormattableString text, Style? normalStyle = null, Style? highlightStyle = null);

    T Prompt<T>(IPrompt<T> prompt);

    void Write(Renderable renderable);

    void WriteAlert(string text);

    void WriteAlertAccent(string text);

    void WriteBlankLine();

    void WriteDim(string text);

    void WriteException(Exception ex);

    void WriteHeading(string text);

    void WriteHeading(string textTemplate, params object?[] args);

    void WriteHeadingWithHighlights(FormattableString text, Style highlightStyle);

    void WriteLine();

    void WriteLine(string text, Style style);

    void WriteNormal(string text);

    void WriteNormal(string textTemplate, params object?[] args);

    void WriteNormalWithHighlights(FormattableString text, Style highlightStyle);

    void WriteRuler();

    void WriteRuler(string heading);

    void WriteSubHeading(string text);
}