using MapifyBackend.database_files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapifyBackend.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestAdminKey = "test-admin-key";

    private readonly string _dbFilePath;

    public CustomWebApplicationFactory()
    {
        _dbFilePath = Path.Combine(Path.GetTempPath(), $"mapify_test_{Guid.NewGuid()}.db");
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
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        try
        {
            if (File.Exists(_dbFilePath))
                File.Delete(_dbFilePath);

            // Clean up default database.db created by Program.cs in test output directory
            string defaultDbPath = Path.Combine(AppContext.BaseDirectory, "database.db");
            if (File.Exists(defaultDbPath))
                File.Delete(defaultDbPath);
        }
        catch
        {
            // best effort cleanup
        }
    }
}
