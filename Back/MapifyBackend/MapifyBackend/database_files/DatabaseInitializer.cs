using Microsoft.Data.Sqlite;
using Dapper;

namespace MapifyBackend.database_files;

public class DatabaseInitializer
{
    private const string DbFileName = "database.db";
    private const string ConnectionString = $"Data Source={DbFileName}";

    public static void EnsureDatabaseCreated()
    {
        EnsureDatabaseCreated(ConnectionString);
    }

    public static void EnsureDatabaseCreated(string connectionString)
    {
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            connection.Execute("PRAGMA FOREIGN_KEYS = ON;");

            var path = Path.Combine(AppContext.BaseDirectory, "database_files", "mainschema.sql");
            string script = File.ReadAllText(path);

            using var command = new SqliteCommand(script, connection);
            command.ExecuteNonQuery();

            Console.WriteLine("Created a database");
        }
        catch (Exception e)
        {
            Console.WriteLine($"DB CREATION FAILED: {e.Message}");
            if (connectionString.StartsWith("Data Source="))
            {
                var fileName = connectionString["Data Source=".Length..];
                if (fileName != ":memory:" && File.Exists(fileName))
                    File.Delete(fileName);
            }
        }
    }
}
