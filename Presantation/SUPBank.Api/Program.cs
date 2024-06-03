using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Serilog;
using SUPBank.Api.Options;
using SUPBank.Application;
using SUPBank.Infrastructure;
using SUPBank.Persistence;

// Initializing the web application builder
var builder = WebApplication.CreateBuilder(args);

// Setting up Serilog logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Registering layer dependencies
builder.Services.AddApplicationDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddPersistenceDependencies();

// Adding controller services
builder.Services.AddControllers();


// Retrieving the environment from configuration
string? environment = builder.Configuration["Environment"];
if (string.IsNullOrEmpty(environment))
{
    throw new InvalidOperationException("Environment not specified in appsettings.json");
}

// Setting the environment name for the application
builder.Environment.EnvironmentName = environment;

// Configuring the app settings and environment variables
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath) // Setting the base path for configuration files => `C:\Users\SUP\Desktop\src\SUPBank\Presantation\SUPBank.Api`
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Adding the main appsettings file
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true) // Adding environment-specific appsettings file
    .AddEnvironmentVariables(); // Adding environment variables

if (environment == "Development")
{
    // Adding endpoint API explorer
    builder.Services.AddEndpointsApiExplorer();

    // Adding Swagger
    builder.Services.AddSwaggerGen();
}


// Adding API versioning
builder.Services.AddApiVersioning(config =>
{
    // Set the default API version to be used when no version is specified in the request.
    config.DefaultApiVersion = new ApiVersion(3, 0);

    // Assume the default API version when the version is not explicitly specified in the request.
    config.AssumeDefaultVersionWhenUnspecified = true;

    // Report API versions in the response headers and supported API versions in response options.
    config.ReportApiVersions = true;

    // Configure the API version reader to use URL segment-based versioning.
    config.ApiVersionReader = new UrlSegmentApiVersionReader();

    // Configure the API version reader to use query string-based versioning.
    // config.ApiVersionReader = new QueryStringApiVersionReader();

    // Configure the API version reader to use header-based versioning.
    // config.ApiVersionReader = new HeaderApiVersionReader();
}).AddMvc().AddApiExplorer(config =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


// Building the web application
var app = builder.Build();

// Configuring middleware for development environment
if (environment == "Development")
{
    // Enabling Swagger middleware
    app.UseSwagger();

    // Enabling Swagger UI
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                description.ApiVersion.ToString());
        }
    });
}

// Enabling authorization middleware
app.UseAuthorization();

// Mapping controller endpoints
app.MapControllers();

// Running the web application
app.Run();

// Flushing and closing the Serilog logger
Log.CloseAndFlush();