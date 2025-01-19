using Spectre.Console;

namespace Safe;

public interface IMySafeConsole
{
    void Write(string text);
    void Markup(string markup);
    string Prompt(TextPrompt<string> prompt);
    void WriteTable(Table table);
}