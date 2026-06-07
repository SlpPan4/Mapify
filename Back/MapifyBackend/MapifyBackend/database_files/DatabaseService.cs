using Dapper;
using Microsoft.Data.Sqlite;

namespace MapifyBackend.database_files;

/// <summary>
/// Service for interacting with the SQLite database using Dapper.
/// Provides CRUD operations for strategies, maps, categories, and operators.
/// </summary>
public class DatabaseService
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="dbFileName">The name or path of the SQLite database file. Defaults to "database.db".</param>
    public DatabaseService(string dbFileName = "database.db")
    {
        // Setting path
        _connectionString = $"Data Source={dbFileName}";
    }

    /// <summary>
    /// Creates and opens a new SQLite database connection.
    /// </summary>
    /// <returns>An open <see cref="SqliteConnection"/> instance.</returns>
    private SqliteConnection GetConnection()
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Retrieves all strategies from the database.
    /// </summary>
    /// <returns>A list of <see cref="Strat"/> objects.</returns>
    public List<Strat> GetAllStrats()
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT id, name, video_url AS videoUrl, map_id AS mapId, description " +
                     "FROM strats";
        return db.Query<Strat>(sql).ToList();
    }

    /// <summary>
    /// Adds a new strategy to the database and assigns the generated ID to the provided object.
    /// </summary>
    /// <param name="strat">The strategy object containing data to insert.</param>
    public void AddStrat(Strat strat)
    {
        using SqliteConnection db = GetConnection();
        string sql = @"INSERT INTO strats (name, video_url, map_id)
                        VALUES (@name, @videoUrl, @mapId);
                        SELECT last_insert_rowid();";
        int newId = db.QuerySingle<int>(sql, new
        {
            name = strat.Name,
            videoUrl = strat.VideoUrl,
            mapId = strat.MapId
        });

        strat.SetId(newId);
    }

    /// <summary>
    /// Retrieves the ID of a map by its unique name.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <returns>The map ID if found; otherwise, null.</returns>
    public int? GetMapIdByName(string mapName)
    {
        using SqliteConnection db = GetConnection();
        
        string sql = @"SELECT id FROM maps WHERE name = @name";
        return db.QuerySingle<int?>(sql, new { name = mapName });
    }

    /// <summary>
    /// Retrieves a single strategy by its ID.
    /// </summary>
    /// <param name="stratId">The ID of the strategy to retrieve.</param>
    /// <returns>A <see cref="Strat"/> object if found; otherwise, null.</returns>
    public Strat? GetStratById(int stratId)
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT id, name, video_url AS videoUrl, map_id AS MapId, description " +
                     "FROM strats " +
                     "WHERE id = @strat_id";
        return db.QuerySingleOrDefault<Strat>(sql, new { strat_id = stratId });
    }

    /// <summary>
    /// Deletes a strategy from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the strategy to delete.</param>
    public void DeleteStrat(int id)
    {
        using SqliteConnection db = GetConnection();
        string sql = "DELETE FROM strats WHERE id=@id";
        db.Execute(sql, new { id = id });
    }
    
    /// <summary>
    /// Retrieves a map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map.</param>
    /// <returns>A <see cref="Map"/> object if found; otherwise, null.</returns>
    public Map? GetMapById(int id)
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT * FROM maps WHERE id=@id";
        return db.QuerySingleOrDefault<Map>(sql, new { id = id });
    }

    /// <summary>
    /// Retrieves all existing categories from the database.
    /// </summary>
    /// <returns>A list of <see cref="Category"/> objects.</returns>
    public List<Category> GetAllCategories()
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT id, name, side FROM categories";
        return db.Query<Category>(sql).ToList();
    }

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category.</param>
    /// <returns>A <see cref="Category"/> object if found; otherwise, null.</returns>
    public Category? GetCategoryById(int id)
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT id, name, side FROM categories WHERE id=@id";
        return db.QuerySingleOrDefault<Category>(sql, new { id = id });
    }

    /// <summary>
    /// Deletes a category from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    public void DeleteCategory(int id)
    {
        using SqliteConnection db = GetConnection();
        string sql = "DELETE FROM categories WHERE id=@id";
        db.Execute(sql, new { id = id });
    }

    /// <summary>
    /// Adds a new category to the database and updates the object with its generated database ID.
    /// </summary>
    /// <param name="category">The category object containing the data to insert.</param>
    public void AddCategory(Category category)
    {
        using SqliteConnection db = GetConnection();
        string sql = "INSERT INTO categories (name, side) " +
                     "VALUES (@name, @side); " +
                     "SELECT last_insert_rowid(); ";
        int newId = db.QuerySingle<int>(sql, new
        {
            name = category.Name,
            side = category.Side.ToString()
        });
        
        category.SetId(newId);
    }

    /// <summary>
    /// Creates a connection between a strategy and a category in the many-to-many table.
    /// </summary>
    /// <param name="stratId">The ID of the strategy.</param>
    /// <param name="categoryId">The ID of the category.</param>
    public void AssignStratToCategory(int stratId, int categoryId)
    {
        using SqliteConnection db = GetConnection();
        string sql = "INSERT INTO strat_categories (strat_id, category_id) " +
                     "VALUES (@stratId, @categoryId)";
        db.Execute(sql, new { stratId = stratId, categoryId = categoryId });
    }

    /// <summary>
    /// Retrieves all strategies associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category.</param>
    /// <returns>A list of <see cref="Strat"/> objects belonging to the category.</returns>
    public List<Strat>? GetStratsByCategory(int categoryId)
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT s.id, s.name, s.video_url AS videoUrl, s.map_id AS mapId, s.description " +
                     "FROM strats s " +
                     "JOIN strat_categories sc ON s.id = sc.strat_id " +
                     "WHERE sc.category_id = @categoryId";
        return db.Query<Strat>(sql, new { categoryId = categoryId }).ToList();
    }

    /// <summary>
    /// Retrieves the name of a category by its ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category.</param>
    /// <returns>The name of the category as a string if found; otherwise, null.</returns>
    public string? GetCategoryNameById(int categoryId)
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT name FROM categories WHERE id=@id";
        return db.QuerySingleOrDefault<string>(sql, new { id = categoryId });
    }
    
    /// <summary>
    /// Creates a connection between a strategy and an operator
    /// in the many-to-many table "strat_operators".
    /// </summary>
    /// <param name="stratId">
    /// The ID of the strategy that the operator should be assigned to.
    /// </param>
    /// <param name="operatorId">
    /// The ID of the operator that should be linked to the strategy.
    /// </param>
    /// <remarks>
    /// This method inserts a new row into the "strat_operators" table.
    /// 
    /// Possible exceptions:
    /// - SQLiteException if the foreign key does not exist
    /// - SQLiteException if the relation already exists and UNIQUE is enforced
    /// - SQLiteException if the SQL query is invalid
    /// 
    /// Requires foreign keys to be enabled in SQLite.
    /// </remarks>
    public bool AssignOperatorToStrat(int stratId, int operatorId)
    {
        using SqliteConnection db = GetConnection();

        string sql = """
                     INSERT INTO strat_operators (strat_id, operator_id)
                     VALUES (@stratId, @operatorId)
                     """;

        try
        {
            int rows = db.Execute(sql, new
            {
                stratId,
                operatorId
            });

            return rows > 0;
        }
        catch (SqliteException)
        {
            // сюда попадёшь при:
            // - foreign key violation
            // - duplicate (если UNIQUE есть)
            return false;
        }
    }
    
    /// <summary>
    /// Checks whether a specific operator is assigned to a specific strategy.
    /// </summary>
    /// <param name="stratId">The ID of the strategy.</param>
    /// <param name="operatorId">The ID of the operator.</param>
    /// <returns>True if the operator is assigned to the strategy; otherwise, false.</returns>
    public bool IsOperatorAssignedToStrat(int stratId, int operatorId)
    {
        using SqliteConnection db = GetConnection();

        string sql = """
                     SELECT 1
                     FROM strat_operators
                     WHERE strat_id=@stratId AND operator_id=@operatorId
                     LIMIT 1
                     """;

        return db.QuerySingleOrDefault<int?>(sql, new { stratId, operatorId }) != null;
    }
    
    /// <summary>
    /// Removes the connection between a strategy and an operator
    /// from the "strat_operators" table.
    /// </summary>
    /// <param name="stratId">
    /// The ID of the strategy.
    /// </param>
    /// <param name="operatorId">
    /// The ID of the operator.
    /// </param>
    /// <returns>
    /// True if a relation was deleted successfully (at least one row affected),
    /// otherwise false (no such relation existed).
    /// </returns>
    /// <remarks>
    /// If the relation does not exist, the query executes successfully
    /// but affects 0 rows, and the method returns false.
    /// </remarks>
    public bool RemoveOperatorFromStrat(int stratId, int operatorId)
    {
        using SqliteConnection db = GetConnection();

        string sql = """
                     DELETE FROM strat_operators
                     WHERE strat_id=@stratId
                       AND operator_id=@operatorId
                     """;

        int rowsAffected = db.Execute(sql, new
        {
            stratId = stratId,
            operatorId = operatorId
        });

        return rowsAffected > 0;
    }
    
    /// <summary>
    /// Retrieves a single operator by its ID.
    /// </summary>
    /// <param name="operatorId">
    /// The ID of the operator to retrieve.
    /// </param>
    /// <returns>
    /// Returns an Operator object if found.
    /// Returns null if no operator exists with the given ID.
    /// </returns>
    /// <remarks>
    /// Uses QuerySingleOrDefault, meaning:
    /// - 0 rows -> null
    /// - 1 row  -> Operator object
    /// - more than 1 row -> exception
    /// </remarks>
    public Operator? GetOperatorById(int operatorId)
    {
        using SqliteConnection db = GetConnection();
    
        string sql = """
                     SELECT id, name, side
                     FROM operators
                     WHERE id=@id
                     """;
    
        return db.QuerySingleOrDefault<Operator>(
            sql,
            new { id = operatorId }
        );
    }
    
    /// <summary>
    /// Retrieves the ID of an operator by its name.
    /// </summary>
    /// <param name="operatorName">
    /// The exact name of the operator.
    /// </param>
    /// <returns>
    /// Returns the operator ID if found.
    /// Returns null if the operator does not exist.
    /// </returns>
    /// <remarks>
    /// This method assumes operator names are unique.
    /// 
    /// Uses nullable int to correctly represent
    /// the absence of a result.
    /// </remarks>
    public int? GetOperatorIdByName(string operatorName)
    {
        using SqliteConnection db = GetConnection();
    
        string sql = """
                     SELECT id
                     FROM operators
                     WHERE name=@operatorName
                     """;
    
        return db.QuerySingleOrDefault<int?>(
            sql,
            new { operatorName = operatorName }
        );
    }

    /// <summary>
    /// Retrieves all operators from the database.
    /// </summary>
    /// <returns>A list of all <see cref="Operator"/> objects.</returns>
    public List<Operator> GetAllOperators()
    {
        using SqliteConnection db = GetConnection();

        string sql = """
                     SELECT id, name, side
                     FROM operators
                     """;
        return db.Query<Operator>(sql).ToList();
    }
}