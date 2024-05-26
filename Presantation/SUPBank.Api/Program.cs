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

var env = builder.Environment; // launchSettings.json "ASPNETCORE_ENVIRONMENT" => `Development` || `Production`
builder.Configuration
    .SetBasePath(env.ContentRootPath) // setting local or server path dynamically => `C:\Users\SUP\Desktop\src\SUPBank\Presantation\SUPBank.Api`
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();