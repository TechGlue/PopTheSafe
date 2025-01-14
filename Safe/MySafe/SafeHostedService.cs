using Microsoft.Extensions.Hosting;

namespace Safe;

public class SafeHostedService : IHostedService
{
    private readonly ISafe _safe;
    private readonly IHostApplicationLifetime _lifetime;

    public SafeHostedService( ISafe safe,
        IHostApplicationLifetime lifetime)
    {
        _safe = safe;
        _lifetime = lifetime;
    }

    private void SafeMenu()
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
            while (true)
            {
                Console.WriteLine("\n" + _safe.Describe());
                Console.WriteLine("What will you do?");
                Console.WriteLine("1) Open the door");
                Console.WriteLine("2) Close the door");
                Console.WriteLine("3) Enter a pin");
                Console.WriteLine("4) Press reset");
                Console.WriteLine("5) Press lock");

                // choice then is the number 
                var action = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice) switch
                {
                    true => actions.ContainsKey(choice) switch
                    {
                        true => actions[choice],
                        false => actions[-1],
                    },
                    false => actions[-1], // this line is hit when we get an error throws a key not found exception
                };

                var result = action(_safe);

                if (!result.isSuccessful)
                {
                    Console.WriteLine($"Failed: {result.isDetail}");
                }
            }
        }
        catch (KeyNotFoundException)
        {
            _lifetime.StopApplication();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Hosted Safe Service Started.");

        Task.Run(() => SafeMenu());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Hosted Safe Service Stopped.");
        return Task.CompletedTask;
    }
}