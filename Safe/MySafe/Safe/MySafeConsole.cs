using Spectre.Console;

namespace Safe;

public class MySafeConsole:IMySafeConsole
{
    private readonly IAnsiConsole _console = AnsiConsole.Console;

    public void Write(string text)
    {
        _console.Write(text);
    }

    public void Markup(string markup)
    {
        _console.Markup(markup);
    }

    public string Prompt(TextPrompt<string> prompt)
    {
        return _console.Prompt(prompt);
    }

    public void WriteTable(Table table)
    {
        _console.Write(table);
    }
}