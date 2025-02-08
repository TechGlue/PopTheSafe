using Serilog;
using MySafe.AdminCodeGenerator;
using MySafe.ErrorHandling;
using MySafe.SafeHelper;

// note: exception handlers are by default only enabled in production
// todo: singleton instance is fine for what we are doing but need to have a way to continuously update the state of the safe on the GUI side. Single ton would be fine for initially getting the GUI set up
// idea inject the safe instance. into the keypad component. leave the ng on init because we want to grabe the most recent status 
// just on every event re update the state. The safe state is fine just need to configure it to be dynamic. 



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

// Re-enable later for logging of exceptions
builder.Services.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
builder.Services.AddSingleton<ISafe, Safe>();
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