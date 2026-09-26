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
            // Инициализация может запускаться параллельно (интеграционные тесты):
            // busy_timeout заставляет конкурента подождать, а транзакция не даёт
            // увидеть полуприменённую схему (иначе гонка = FOREIGN KEY constraint failed)
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
            // Не пытаемся "починить" БД удалением файла: при битой схеме падаем
            // сразу и с полным стеком, а данные остаются на месте для разбирательства.
            Console.WriteLine($"DB CREATION FAILED [{connectionString}]: {e}");
            throw;
        }
    }
}
