using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Safe;

// Accept the default builder
// But, disable logging. For now, we'll use the console
IHostBuilder hostBuilder = Host.CreateDefaultBuilder().ConfigureLogging((logging) => logging.ClearProviders());

// Inject what we need for SafeHostedService 
hostBuilder.ConfigureServices(serviceCollection =>
{
    serviceCollection.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
    serviceCollection.AddSingleton<ISafe, MySafe>();
    serviceCollection.AddHostedService<SafeHostedService>();
});

await hostBuilder.RunConsoleAsync();