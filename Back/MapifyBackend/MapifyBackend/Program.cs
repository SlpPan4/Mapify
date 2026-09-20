using System.Text.Json.Serialization;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;

var builder = WebApplication.CreateBuilder(args);

// Initialize db
string connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=database.db";
DatabaseInitializer.EnsureDatabaseCreated(connectionString);

bool isDevelopment = builder.Environment.IsDevelopment();

// Register services
builder.Services.AddSingleton(_ => new DatabaseService(connectionString)); // One for all time
builder.Services.AddScoped<StratService>();     // Gets created for each request
builder.Services.AddScoped<CategoryService>(); // same
builder.Services.AddScoped<OperatorService>();
builder.Services.AddScoped<MapService>();
builder.Services.AddScoped<SubmissionService>();

// add Cors and controllers support
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", p =>
    {
        if (allowedOrigins.Length > 0)
        {
            p.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
        }
        else if (isDevelopment)
        {
            // Convenience for local dev when no origins are configured.
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
        // Production with no configured origins: cross-origin requests are denied by default.
    });
});

var app = builder.Build();

// Resolve admin API key after Build() so test/host-level configuration overrides are visible.
// From config in every environment; the dev fallback applies only in Development.
string? adminApiKey = app.Configuration["AdminApi:Key"];

if (string.IsNullOrEmpty(adminApiKey))
{
    if (!isDevelopment)
        throw new InvalidOperationException(
            "Admin API key is not configured. Set the AdminApi__Key environment variable.");
    adminApiKey = "mapify-dev-admin-key";
}

if (!isDevelopment && adminApiKey == "mapify-dev-admin-key")
    throw new InvalidOperationException(
        "The dev admin API key must not be used outside Development. Set a unique AdminApi__Key.");

// Middleware
app.UseCors("AppCors");
app.UseMiddleware<ApiKeyAuthMiddleware>(adminApiKey);
app.MapControllers(); // Connects URL with controllers

// Run server
app.Run();

public partial class Program { }
