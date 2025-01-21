using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Safe;

// IHosted service is not good at having long-running tasks in the StartAsync because the application will not fully startup
// Avoid long-running tasks in Start Async 
public class SafeHostedService : BackgroundService
{
    private readonly ISafe _safe;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IMySafeConsole _console;
    private readonly ILogger<SafeHostedService> _logger;

    public SafeHostedService(ISafe safe,
        IHostApplicationLifetime lifetime, MySafeConsole console, ILogger<SafeHostedService> logger)
    {
        _console = console;
        _safe = safe;
        _lifetime = lifetime;
        _logger = logger;
    }

    private Task<bool> SafeMenu()
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
            Console.WriteLine("\n" + _safe.Describe());
            Console.WriteLine("What will you do?");
            Console.WriteLine("1) Open the door");
            Console.WriteLine("2) Close the door");
            Console.WriteLine("3) Enter a pin");
            Console.WriteLine("4) Press reset");
            Console.WriteLine("5) Press lock");


            var action = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice) switch
            {
                true => actions.ContainsKey(choice) switch
                {
                    true => actions[choice],
                    false => actions[-1],
                },
                false => actions[-1],
            };

            var result = action(_safe);

            if (!result.isSuccessful)
            {
                Console.WriteLine($"Failed: {result.isDetail}");
            }
        }
        catch (KeyNotFoundException)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }


    // This is where long-running loads should be stored
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Await for 200ms to ensure startup
        await Task.Delay(200, stoppingToken);

        // Loop menu
        bool activeMenu = true;

        while (!stoppingToken.IsCancellationRequested && activeMenu)
        {
            activeMenu = await SafeMenu();
        }

        _lifetime.StopApplication();
    }
}