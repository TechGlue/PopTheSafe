using Serilog;
using MySafe.AdminCodeGenerator;
using MySafe.ErrorHandling;
using MySafe.SafeHelper;


/*Todo: 
 *
 * 
 */

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


// in-memory storage
builder.Services.AddSingleton<SafeCache>();
builder.Services.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
builder.Services.AddScoped<ISafe, Safe>();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<SafeErrorHandling>();
builder.Services.AddProblemDetails();

// enable cross-origin requests only from localhost:4200 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( policy =>
    {
        string temp = builder.Configuration.GetValue<string>("CORS:ContainerHost") ?? string.Empty;
        policy.WithOrigins(temp);
        // policy enables any http methods and headers from client
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapControllers();
app.UseCors();
app.UseExceptionHandler();

app.Run();