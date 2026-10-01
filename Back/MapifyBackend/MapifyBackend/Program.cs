using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
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
builder.Services.AddScoped<BombsiteService>();
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

// Rate limiting for the public submission endpoints (anti-spam).
// Limits come from config (RateLimiting:Submissions) but are resolved per request
// from DI, so integration tests can override them via the service collection.
builder.Services.AddSingleton(_ =>
    builder.Configuration.GetSection("RateLimiting:Submissions").Get<SubmissionRateLimitOptions>()
    ?? new SubmissionRateLimitOptions());
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("submissions", httpContext =>
    {
        var limits = httpContext.RequestServices.GetRequiredService<SubmissionRateLimitOptions>();
        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limits.PermitLimit,
            Window = TimeSpan.FromSeconds(limits.WindowSeconds),
            QueueLimit = 0
        });
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
app.UseRateLimiter();
app.UseMiddleware<ApiKeyAuthMiddleware>(adminApiKey);
app.MapControllers(); // Connects URL with controllers

// Run server
app.Run();

public partial class Program { }
