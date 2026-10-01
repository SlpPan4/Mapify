using Microsoft.Data.Sqlite;
using Dapper;

namespace MapifyBackend.database_files;

public class DatabaseInitializer
{
    private static readonly string[] DefaultBombsiteNames = ["Site A", "Site B", "Site C", "Site D", "All"];

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

            ApplyColumnMigrations(connection, transaction);
            SeedBombsites(connection, transaction);

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

    // CREATE TABLE IF NOT EXISTS cannot add columns to tables that already exist,
    // so existing databases get the new nullable columns via ALTER TABLE.
    private static void ApplyColumnMigrations(SqliteConnection connection, SqliteTransaction transaction)
    {
        EnsureColumn(connection, transaction, "strats", "bombsite_id",
            "bombsite_id INT REFERENCES bombsites(id)");
        EnsureColumn(connection, transaction, "pending_strat_submissions", "bombsite_id",
            "bombsite_id INT REFERENCES bombsites(id)");
    }

    private static void EnsureColumn(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string table,
        string column,
        string columnDefinition)
    {
        var columns = connection
            .Query<string>($"SELECT name FROM pragma_table_info('{table}')", transaction: transaction)
            .ToList();

        if (!columns.Contains(column))
        {
            connection.Execute($"ALTER TABLE {table} ADD COLUMN {columnDefinition}", transaction: transaction);
        }
    }

    // Every map gets the default bombsites; INSERT OR IGNORE keeps this idempotent.
    private static void SeedBombsites(SqliteConnection connection, SqliteTransaction transaction)
    {
        var mapIds = connection.Query<int>("SELECT id FROM maps", transaction: transaction).ToList();

        foreach (int mapId in mapIds)
        {
            foreach (string name in DefaultBombsiteNames)
            {
                connection.Execute(
                    "INSERT OR IGNORE INTO bombsites (map_id, name) VALUES (@mapId, @name)",
                    new { mapId, name },
                    transaction: transaction);
            }
        }
    }
}
