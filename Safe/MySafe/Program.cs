using Serilog;
using MySafe.AdminCodeGenerator;
using MySafe.ErrorHandling;
using MySafe.SafeHelper;

// note: exception handlers are by default only enabled in production

var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

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

// Re-enable later for logging of exceptions
builder.Services.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
builder.Services.AddTransient<ISafe, Safe>();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<SafeErrorHandling>();
builder.Services.AddProblemDetails();

// enable cross-origin requests only from localhost:4200 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins, policy =>
    {
        string temp = builder.Configuration.GetValue<string>("CORS:ContainerHost") ?? string.Empty;
        policy.WithOrigins(temp);
    });
});

var app = builder.Build();

app.MapControllers();
app.UseCors(myAllowSpecificOrigins);
app.UseExceptionHandler();

app.Run();