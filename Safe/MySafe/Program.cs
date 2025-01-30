using Safe;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// two-step initialization 
// first step
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// seconds step, add callback
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
builder.Services.AddSingleton<ISafe, MySafe>();

// Todo: not sure what we are going to do wiwth this now. move the service to an endpoint
// builder.Services.AddHostedService<SafeHostedService>();

var app = builder.Build();

app.MapGet("/", () => "Safe API is running");

await app.RunAsync();