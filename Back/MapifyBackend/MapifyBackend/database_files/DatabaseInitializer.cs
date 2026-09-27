using Microsoft.Data.Sqlite;
using Dapper;

namespace MapifyBackend.database_files;

public class DatabaseInitializer
{
    public static void EnsureDatabaseCreated(string connectionString)
    {
        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            connection.Execute("PRAGMA FOREIGN_KEYS = ON;");
            // Initialization can run concurrently (integration tests):
            // busy_timeout makes a concurrent initializer wait, and the transaction
            // prevents it from seeing a half-applied schema (a race = FOREIGN KEY constraint failed)
            connection.Execute("PRAGMA busy_timeout = 10000;");

            var path = Path.Combine(AppContext.BaseDirectory, "database_files", "mainschema.sql");
            string script = File.ReadAllText(path);

            using var transaction = connection.BeginTransaction();
            using var command = new SqliteCommand(script, connection, transaction);
            command.ExecuteNonQuery();
            transaction.Commit();

            Console.WriteLine("Created a database");
        }
        catch (Exception e)
        {
            // Do not "fix" the database by deleting the file: on a broken schema we fail
            // fast with the full stack trace, and the data stays in place for investigation.
            Console.WriteLine($"DB CREATION FAILED [{connectionString}]: {e}");
            throw;
        }
    }
}
