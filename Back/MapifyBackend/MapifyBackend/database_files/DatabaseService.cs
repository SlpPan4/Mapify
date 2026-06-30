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
        connection.Execute("PRAGMA FOREIGN_KEYS = ON;");
        return connection;
    }

    /// <summary>
    /// Retrieves all strategies from the database.
    /// </summary>
    /// <returns>A list of <see cref="Strat"/> objects.</returns>
    public async Task<List<Strat>> GetAllStrats()
    {
        await using SqliteConnection db = GetConnection();
        string sql = "SELECT id, name, video_url AS videoUrl, map_id AS mapId, description " +
                     "FROM strats";
        var result = await db.QueryAsync<Strat>(sql);
        return result.ToList();
    }

    /// <summary>
    /// Adds a new strategy to the database and assigns the generated ID to the provided object.
    /// </summary>
    /// <param name="strat">The strategy object containing data to insert.</param>
    public async Task<int> AddStrat(Strat strat)
    {
        await using SqliteConnection db = GetConnection();

        string sql = @"INSERT INTO strats (name, video_url, map_id, description)
                   VALUES (@name, @videoUrl, @mapId, @description);
                   SELECT last_insert_rowid();";

        return await db.QuerySingleAsync<int>(sql, new
        {
            name = strat.Name,
            videoUrl = strat.VideoUrl,
            mapId = strat.MapId,
            description = strat.Description
        });
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
        return db.QuerySingleOrDefault<int?>(sql, new { name = mapName });
    }

    /// <summary>
    /// Retrieves a single strategy by its ID.
    /// </summary>
    /// <param name="stratId">The ID of the strategy to retrieve.</param>
    /// <returns>A <see cref="Strat"/> object if found; otherwise, null.</returns>
    public async Task<Strat?> GetStratById(int stratId)
    {
        await using SqliteConnection db = GetConnection();
        string sql = "SELECT id, name, video_url AS videoUrl, map_id AS MapId, description " +
                     "FROM strats " +
                     "WHERE id = @strat_id";
        var result = await db.QuerySingleOrDefaultAsync(sql, new { strat_id = stratId });
        return result;
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
    /// Retrieves the ID of a category by its unique name.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <returns>The category ID if found; otherwise, null.</returns>
    public int? GetCategoryIdByName(string categoryName)
    {
        using SqliteConnection db = GetConnection();
        string sql = "SELECT id FROM categories WHERE name=@name";
        return db.QuerySingleOrDefault<int?>(sql, new { name = categoryName });
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
    /// Adds a category proposal to the pending submissions table.
    /// </summary>
    /// <param name="submission">The category submission to store for review.</param>
    /// <returns>The generated pending submission ID.</returns>
    public int AddPendingCategorySubmission(CategorySubmission submission)
    {
        using SqliteConnection db = GetConnection();
        string sql = """
                     INSERT INTO pending_category_submissions (name, side)
                     VALUES (@name, @side);
                     SELECT last_insert_rowid();
                     """;

        return db.QuerySingle<int>(sql, new
        {
            name = submission.Name,
            side = submission.Side.ToString()
        });
    }

    /// <summary>
    /// Retrieves all category submissions waiting for admin review.
    /// </summary>
    /// <returns>A list of pending category submissions.</returns>
    public List<CategorySubmission> GetPendingCategorySubmissions()
    {
        using SqliteConnection db = GetConnection();
        string sql = """
                     SELECT id, name, side, submitted_at AS SubmittedAt
                     FROM pending_category_submissions
                     ORDER BY submitted_at ASC, id ASC
                     """;

        return db.Query<CategorySubmission>(sql).ToList();
    }

    /// <summary>
    /// Retrieves a single category submission by ID.
    /// </summary>
    /// <param name="id">The pending category submission ID.</param>
    /// <returns>The pending submission if found; otherwise, null.</returns>
    public CategorySubmission? GetPendingCategorySubmissionById(int id)
    {
        using SqliteConnection db = GetConnection();
        string sql = """
                     SELECT id, name, side, submitted_at AS SubmittedAt
                     FROM pending_category_submissions
                     WHERE id=@id
                     """;

        return db.QuerySingleOrDefault<CategorySubmission>(sql, new { id });
    }

    /// <summary>
    /// Approves a pending category submission and moves it into the categories table.
    /// </summary>
    /// <param name="id">The pending category submission ID.</param>
    /// <returns>The generated category ID if approved; otherwise, null.</returns>
    public int? ApprovePendingCategorySubmission(int id)
    {
        using SqliteConnection db = GetConnection();
        using var transaction = db.BeginTransaction();

        CategorySubmission? submission = db.QuerySingleOrDefault<CategorySubmission>(
            """
            SELECT id, name, side, submitted_at AS SubmittedAt
            FROM pending_category_submissions
            WHERE id=@id
            """,
            new { id },
            transaction);

        if (submission == null)
        {
            transaction.Rollback();
            return null;
        }

        int categoryId = db.QuerySingle<int>(
            """
            INSERT INTO categories (name, side)
            VALUES (@name, @side);
            SELECT last_insert_rowid();
            """,
            new
            {
                name = submission.Name,
                side = submission.Side.ToString()
            },
            transaction);

        db.Execute(
            "DELETE FROM pending_category_submissions WHERE id=@id",
            new { id },
            transaction);

        transaction.Commit();
        return categoryId;
    }

    /// <summary>
    /// Deletes a category submission without approving it.
    /// </summary>
    /// <param name="id">The pending category submission ID.</param>
    /// <returns>True if a row was deleted; otherwise, false.</returns>
    public bool DeletePendingCategorySubmission(int id)
    {
        using SqliteConnection db = GetConnection();
        int rows = db.Execute("DELETE FROM pending_category_submissions WHERE id=@id", new { id });
        return rows > 0;
    }

    /// <summary>
    /// Adds a strategy proposal and its proposed category/operator assignments.
    /// </summary>
    /// <param name="submission">The strategy submission to store for review.</param>
    /// <returns>The generated pending submission ID.</returns>
    public int AddPendingStratSubmission(StratSubmission submission)
    {
        using SqliteConnection db = GetConnection();
        using var transaction = db.BeginTransaction();

        int submissionId = db.QuerySingle<int>(
            """
            INSERT INTO pending_strat_submissions (name, video_url, map_id, description)
            VALUES (@name, @videoUrl, @mapId, @description);
            SELECT last_insert_rowid();
            """,
            new
            {
                name = submission.Name,
                videoUrl = submission.VideoUrl,
                mapId = submission.MapId,
                description = submission.Description
            },
            transaction);

        foreach (int categoryId in submission.CategoryIds.Distinct())
        {
            db.Execute(
                """
                INSERT INTO pending_strat_submission_categories (submission_id, category_id)
                VALUES (@submissionId, @categoryId)
                """,
                new { submissionId, categoryId },
                transaction);
        }

        foreach (int operatorId in submission.OperatorIds.Distinct())
        {
            db.Execute(
                """
                INSERT INTO pending_strat_submission_operators (submission_id, operator_id)
                VALUES (@submissionId, @operatorId)
                """,
                new { submissionId, operatorId },
                transaction);
        }

        transaction.Commit();
        return submissionId;
    }

    /// <summary>
    /// Retrieves all strategy submissions waiting for admin review.
    /// </summary>
    /// <returns>A list of pending strategy submissions.</returns>
    public List<StratSubmission> GetPendingStratSubmissions()
    {
        using SqliteConnection db = GetConnection();
        string sql = """
                     SELECT id, name, video_url AS VideoUrl, map_id AS MapId, description, submitted_at AS SubmittedAt
                     FROM pending_strat_submissions
                     ORDER BY submitted_at ASC, id ASC
                     """;

        List<StratSubmission> submissions = db.Query<StratSubmission>(sql).ToList();
        foreach (StratSubmission submission in submissions)
        {
            LoadPendingStratSubmissionRelations(db, submission);
        }

        return submissions;
    }

    /// <summary>
    /// Retrieves a single strategy submission by ID.
    /// </summary>
    /// <param name="id">The pending strategy submission ID.</param>
    /// <returns>The pending submission if found; otherwise, null.</returns>
    public StratSubmission? GetPendingStratSubmissionById(int id)
    {
        using SqliteConnection db = GetConnection();
        StratSubmission? submission = db.QuerySingleOrDefault<StratSubmission>(
            """
            SELECT id, name, video_url AS VideoUrl, map_id AS MapId, description, submitted_at AS SubmittedAt
            FROM pending_strat_submissions
            WHERE id=@id
            """,
            new { id });

        if (submission == null) return null;

        LoadPendingStratSubmissionRelations(db, submission);
        return submission;
    }

    /// <summary>
    /// Approves a pending strategy submission and moves it into the public strategy tables.
    /// </summary>
    /// <param name="id">The pending strategy submission ID.</param>
    /// <returns>The generated strategy ID if approved; otherwise, null.</returns>
    public int? ApprovePendingStratSubmission(int id)
    {
        using SqliteConnection db = GetConnection();
        using var transaction = db.BeginTransaction();

        StratSubmission? submission = db.QuerySingleOrDefault<StratSubmission>(
            """
            SELECT id, name, video_url AS VideoUrl, map_id AS MapId, description, submitted_at AS SubmittedAt
            FROM pending_strat_submissions
            WHERE id=@id
            """,
            new { id },
            transaction);

        if (submission == null)
        {
            transaction.Rollback();
            return null;
        }

        int stratId = db.QuerySingle<int>(
            """
            INSERT INTO strats (name, video_url, map_id, description)
            VALUES (@name, @videoUrl, @mapId, @description);
            SELECT last_insert_rowid();
            """,
            new
            {
                name = submission.Name,
                videoUrl = submission.VideoUrl,
                mapId = submission.MapId,
                description = submission.Description
            },
            transaction);

        IEnumerable<int> categoryIds = db.Query<int>(
            """
            SELECT category_id
            FROM pending_strat_submission_categories
            WHERE submission_id=@id
            """,
            new { id },
            transaction);

        foreach (int categoryId in categoryIds)
        {
            db.Execute(
                """
                INSERT INTO strat_categories (strat_id, category_id)
                VALUES (@stratId, @categoryId)
                """,
                new { stratId, categoryId },
                transaction);
        }

        IEnumerable<int> operatorIds = db.Query<int>(
            """
            SELECT operator_id
            FROM pending_strat_submission_operators
            WHERE submission_id=@id
            """,
            new { id },
            transaction);

        foreach (int operatorId in operatorIds)
        {
            db.Execute(
                """
                INSERT INTO strat_operators (strat_id, operator_id)
                VALUES (@stratId, @operatorId)
                """,
                new { stratId, operatorId },
                transaction);
        }

        db.Execute("DELETE FROM pending_strat_submissions WHERE id=@id", new { id }, transaction);

        transaction.Commit();
        return stratId;
    }

    /// <summary>
    /// Deletes a strategy submission without approving it.
    /// </summary>
    /// <param name="id">The pending strategy submission ID.</param>
    /// <returns>True if a row was deleted; otherwise, false.</returns>
    public bool DeletePendingStratSubmission(int id)
    {
        using SqliteConnection db = GetConnection();
        int rows = db.Execute("DELETE FROM pending_strat_submissions WHERE id=@id", new { id });
        return rows > 0;
    }

    private static void LoadPendingStratSubmissionRelations(SqliteConnection db, StratSubmission submission)
    {
        submission.CategoryIds = db.Query<int>(
            """
            SELECT category_id
            FROM pending_strat_submission_categories
            WHERE submission_id=@submissionId
            ORDER BY category_id
            """,
            new { submissionId = submission.Id }).ToList();

        submission.OperatorIds = db.Query<int>(
            """
            SELECT operator_id
            FROM pending_strat_submission_operators
            WHERE submission_id=@submissionId
            ORDER BY operator_id
            """,
            new { submissionId = submission.Id }).ToList();
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
            // caught upon:
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
