using Serilog;
using SUPBank.Application;
using SUPBank.Infrastructure;
using SUPBank.Persistence;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddPersistenceDependencies();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

string? environment = builder.Configuration["Environment"];
if (string.IsNullOrEmpty(environment))
{
    throw new InvalidOperationException("Environment not specified in appsettings.json");
}
builder.Environment.EnvironmentName = environment;
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath) // setting local or server path dynamically => `C:\Users\SUP\Desktop\src\SUPBank\Presantation\SUPBank.Api`
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

if (environment == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();