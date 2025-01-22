using Spectre.Console;

namespace Safe;

public class MySafeConsole : IMySafeConsole
{
    private readonly IAnsiConsole _console = AnsiConsole.Console;

    public int SafeMenu(ISafe safe)
    {
        var actions = new Dictionary<int, Func<ISafe, SafeResponse>>()
        {
            { 1, safe => safe.Open() },
            { 2, safe => safe.Close() },
            {
                3, safe =>
                {
                    Console.Write("\nEnter Safe PIN: ");
                    var pin = Console.ReadLine() ?? "";
                    SafeResponse safeResponse = new SafeResponse();

                    safe.SetCode(pin, result => safeResponse = result);
                    return safeResponse;
                }
            },
            { 4, safe => safe.PressReset() },
            { 5, safe => safe.PressLock() },
        };

        try
        {
            _console.WriteLine("\n" + safe.Describe());
            _console.WriteLine("What will you do?");
            _console.WriteLine("1) Open the door");
            _console.WriteLine("2) Close the door");
            _console.WriteLine("3) Enter a pin");
            _console.WriteLine("4) Press reset");
            _console.WriteLine("5) Press lock");


            var action = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice) switch
            {
                true => actions.ContainsKey(choice) switch
                {
                    true => actions[choice],
                    false => actions[-1],
                },
                false => actions[-1],
            };

            var result = action(safe);

            if (!result.IsSuccessful)
            {
                Console.WriteLine($"Failed: {result.IsDetail}");
            }
        }
        catch (KeyNotFoundException)
        {
            return -1;
        }

        return 0;
    }
}