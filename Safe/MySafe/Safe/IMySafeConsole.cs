using Spectre.Console;

namespace Safe;

public interface IMySafeConsole
{
    bool SafeMenu(ISafe safe);
}