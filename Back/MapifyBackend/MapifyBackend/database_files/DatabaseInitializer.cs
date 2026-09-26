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

            var path = Path.Combine(AppContext.BaseDirectory, "database_files", "mainschema.sql");
            string script = File.ReadAllText(path);

            using var command = new SqliteCommand(script, connection);
            command.ExecuteNonQuery();

            Console.WriteLine("Created a database");
        }
        catch (Exception e)
        {
            // Не пытаемся "починить" БД удалением файла: при битой схеме падаем
            // сразу и с полным стеком, а данные остаются на месте для разбирательства.
            Console.WriteLine($"DB CREATION FAILED: {e}");
            throw;
        }
    }
}
