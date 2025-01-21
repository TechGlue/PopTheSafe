using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Safe;
using Serilog;

// Todo: move menu that's printed to MySafeConsole

IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

// two-step initialization 
// first step
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

hostBuilder.ConfigureServices(serviceCollection =>
{
    // seconds step, add callback
    serviceCollection.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());
    
    
    // Inject the spectre console logger for the safe
    serviceCollection.AddSingleton<MySafeConsole>();
    
    serviceCollection.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
    serviceCollection.AddSingleton<ISafe, MySafe>();
    
    serviceCollection.AddHostedService<SafeHostedService>();
});

await hostBuilder.RunConsoleAsync();