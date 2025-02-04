using Serilog;
using MySafe.AdminCodeGenerator;
using MySafe.SafeHelper;


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

builder.Services.AddSingleton<IAdminCodeGenerator, AdminCodeGenerator>();
builder.Services.AddSingleton<ISafe, Safe>();
builder.Services.AddControllers();

// rethink this for interacting with a safe research the different types of scopes 

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

app.Run();