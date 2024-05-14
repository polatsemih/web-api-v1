using VkBank.Application;
using VkBank.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var env = builder.Environment; // launchSettings.json "ASPNETCORE_ENVIRONMENT" => `Development` || `Production`
builder.Configuration
    .SetBasePath(env.ContentRootPath) // setting local or server path dynamically => `C:\Users\SUP\Desktop\src\VkBank\Presantation\VkBank.Api`
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
