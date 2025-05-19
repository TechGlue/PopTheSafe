using MySafe.SafeHelper;

namespace MySafe;

public class SafeHostedService : BackgroundService
{
    private readonly ISafe _safe;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IMySafeConsole _console;
    private readonly ILogger<SafeHostedService> _logger;

    public SafeHostedService(ISafe safe,
        IHostApplicationLifetime lifetime, SafeConsole console, ILogger<SafeHostedService> logger)
    {
        _console = console;
        _safe = safe;
        _lifetime = lifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 200ms delay to ensure application start up logs
        await Task.Delay(200, stoppingToken);

        await Task.Yield();
        while (!stoppingToken.IsCancellationRequested && _console.SafeMenu(_safe) != -1) ;

        _lifetime.StopApplication();
    }
}