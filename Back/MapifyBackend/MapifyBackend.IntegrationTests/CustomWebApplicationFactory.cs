using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapifyBackend.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestAdminKey = "test-admin-key";

    private readonly string _dbFilePath;
    private readonly SubmissionRateLimitOptions _rateLimitOptions;

    public CustomWebApplicationFactory(int submissionPermitLimit = 1_000_000, int submissionWindowSeconds = 60)
    {
        _dbFilePath = Path.Combine(Path.GetTempPath(), $"mapify_test_{Guid.NewGuid()}.db");
        // The limit is effectively disabled by default so existing tests do not hit it
        // (all requests share one IP). Rate-limit tests pass their own values.
        _rateLimitOptions = new SubmissionRateLimitOptions
        {
            PermitLimit = submissionPermitLimit,
            WindowSeconds = submissionWindowSeconds
        };
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminApi:Key"] = TestAdminKey
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove default DatabaseService registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DatabaseService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register test DatabaseService with isolated temp file
            string connectionString = $"Data Source={_dbFilePath}";
            services.AddSingleton(_ => new DatabaseService(connectionString));

            // Initialize test database
            DatabaseInitializer.EnsureDatabaseCreated(connectionString);

            // Override rate limits for the test app instance (last registration wins)
            services.AddSingleton(_rateLimitOptions);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        try
        {
            if (File.Exists(_dbFilePath))
                File.Delete(_dbFilePath);
            // Do not delete the shared database.db created by Program.cs here: on Linux
            // unlinking an open file breaks parallel factories (SQLITE_READONLY_DBMOVED)
        }
        catch
        {
            // best effort cleanup
        }
    }
}
