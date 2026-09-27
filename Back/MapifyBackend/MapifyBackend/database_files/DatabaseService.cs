using Dapper;
using MapifyBackend.Utility.DTOs;
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
    /// <param name="connectionString">The SQLite connection string.</param>
    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and opens a new SQLite database connection asynchronously.
    /// </summary>
    /// <returns>An open <see cref="SqliteConnection"/> instance.</returns>
    private async Task<SqliteConnection> GetConnectionAsync()
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("PRAGMA FOREIGN_KEYS = ON;");
        return connection;
    }

    /// <summary>
    /// Retrieves strategies filtered by name, map, category, and/or operator.
    /// All filters are optional and combined with AND.
    /// </summary>
    /// <param name="name">Optional case-insensitive substring match on strategy name.</param>
    /// <param name="mapId">Optional filter by map ID.</param>
    /// <param name="categoryId">Optional filter by assigned category ID.</param>
    /// <param name="operatorId">Optional filter by assigned operator ID.</param>
    /// <returns>A list of strategies matching the filters.</returns>
    public async Task<List<Strat>> GetStratsFiltered(string? name, int? mapId, int? categoryId, int? operatorId)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(name))
        {
            conditions.Add("LOWER(s.name) LIKE LOWER(@name)");
            parameters.Add("name", $"%{name}%");
        }

        if (mapId.HasValue)
        {
            conditions.Add("s.map_id = @mapId");
            parameters.Add("mapId", mapId.Value);
        }

        if (categoryId.HasValue)
        {
            conditions.Add("EXISTS (SELECT 1 FROM strat_categories sc WHERE sc.strat_id = s.id AND sc.category_id = @categoryId)");
            parameters.Add("categoryId", categoryId.Value);
        }

        if (operatorId.HasValue)
        {
            conditions.Add("EXISTS (SELECT 1 FROM strat_operators so WHERE so.strat_id = s.id AND so.operator_id = @operatorId)");
            parameters.Add("operatorId", operatorId.Value);
        }

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;
        string sql = $"SELECT s.id, s.name, s.video_url AS videoUrl, s.map_id AS mapId, s.description FROM strats s {whereClause} ORDER BY s.id";

        return (await db.QueryAsync<Strat>(sql, parameters)).ToList();
    }

    /// <summary>
    /// Adds a new strategy to the database and assigns the generated ID to the provided object.
    /// </summary>
    /// <param name="strat">The strategy object containing data to insert.</param>
    public async Task<int> AddStrat(Strat strat)
    {
        await using SqliteConnection db = await GetConnectionAsync();

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
    /// Retrieves all strats by an operator from the database
    /// </summary>
    /// <param name="operatorId"></param>
    /// <returns>A task with a list of Strat objects</returns>
    public async Task<List<Strat>?> StratsByOperator(int operatorId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = @"SELECT id, name, video_url AS videoUrl, map_id AS mapId, description 
                        FROM strats
                        JOIN strat_operators ON strats.id = strat_operators.strat_id 
                        WHERE strat_operators.operator_id = @operatorId";
        var result = await db.QueryAsync<Strat>(sql, new { operatorId });
        return result.ToList();
    }

    /// <summary>
    /// Retrieves all strats by a map from the database
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns>A task with a list of Strat objects</returns>
    public async Task<List<Strat>?> StratsByMapId(int mapId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = @"SELECT id, name, video_url AS videoUrl, map_id AS mapId, description FROM strats WHERE map_id = @mapId";
        var result = await db.QueryAsync<Strat>(sql, new { mapId });
        return result.ToList();
    }

    /// <summary>
    /// Retrieves the ID of a map by its unique name.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <returns>The map ID if found; otherwise, null.</returns>
    public async Task<int?> GetMapIdByName(string mapName)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = @"SELECT id FROM maps WHERE LOWER(name) = LOWER(@name)";
        return await db.QuerySingleOrDefaultAsync<int?>(sql, new { name = mapName });
    }

    /// <summary>
    /// Retrieves a single strategy by its ID.
    /// </summary>
    /// <param name="stratId">The ID of the strategy to retrieve.</param>
    /// <returns>A <see cref="Strat"/> object if found; otherwise, null.</returns>
    public async Task<Strat?> GetStratById(int stratId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id, name, video_url AS videoUrl, map_id AS mapId, description " +
                     "FROM strats " +
                     "WHERE id = @strat_id";
        var result = await db.QuerySingleOrDefaultAsync<Strat>(sql, new { strat_id = stratId });
        return result;
    }

    /// <summary>
    /// Retrieves a single strategy with its map, categories, and operators.
    /// </summary>
    /// <param name="stratId">The ID of the strategy to retrieve.</param>
    /// <returns>A <see cref="StratDetail"/> object if found; otherwise, null.</returns>
    public async Task<StratDetail?> GetStratDetailById(int stratId)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        Strat? strat = await GetStratById(stratId);
        if (strat == null) return null;

        Map? map = await GetMapById(strat.MapId);

        List<Category> categories = (await db.QueryAsync<Category>(
            """
            SELECT c.id, c.name, c.side
            FROM categories c
            JOIN strat_categories sc ON c.id = sc.category_id
            WHERE sc.strat_id = @stratId
            ORDER BY c.name
            """,
            new { stratId })).ToList();

        List<Operator> operators = (await db.QueryAsync<Operator>(
            """
            SELECT o.id, o.name, o.side
            FROM operators o
            JOIN strat_operators so ON o.id = so.operator_id
            WHERE so.strat_id = @stratId
            ORDER BY o.name
            """,
            new { stratId })).ToList();

        return new StratDetail
        {
            Id = strat.Id,
            Name = strat.Name,
            VideoUrl = strat.VideoUrl,
            Description = strat.Description,
            Map = map!,
            Categories = categories,
            Operators = operators
        };
    }

    /// <summary>
    /// Retrieves all strategies enriched with map, side, categories, and operators.
    /// </summary>
    /// <returns>A list of <see cref="StratSummary"/> objects.</returns>
    public async Task<List<StratSummary>> GetStratsSummary()
    {
        await using SqliteConnection db = await GetConnectionAsync();

        List<Strat> strats = (await db.QueryAsync<Strat>(
            "SELECT id, name, video_url AS videoUrl, map_id AS mapId, description FROM strats ORDER BY id")).ToList();

        List<Map> maps = (await db.QueryAsync<Map>("SELECT id, name FROM maps")).ToList();
        List<Category> allCategories = (await db.QueryAsync<Category>("SELECT id, name, side FROM categories")).ToList();
        List<Operator> allOperators = (await db.QueryAsync<Operator>("SELECT id, name, side FROM operators")).ToList();

        Dictionary<int, List<Category>> stratCategories = (await db.QueryAsync<(int stratId, int categoryId)>(
            "SELECT strat_id, category_id FROM strat_categories"))
            .GroupBy(x => x.stratId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => allCategories.First(c => c.Id == x.categoryId)).ToList());

        Dictionary<int, List<Operator>> stratOperators = (await db.QueryAsync<(int stratId, int operatorId)>(
            "SELECT strat_id, operator_id FROM strat_operators"))
            .GroupBy(x => x.stratId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => allOperators.First(o => o.Id == x.operatorId)).ToList());

        Dictionary<int, Map> mapById = maps.ToDictionary(m => m.Id);

        return strats.Select(s =>
        {
            stratCategories.TryGetValue(s.Id, out List<Category>? cats);
            stratOperators.TryGetValue(s.Id, out List<Operator>? ops);
            string side = cats?.FirstOrDefault()?.Side.ToString()
                ?? ops?.FirstOrDefault()?.Side.ToString()
                ?? "Attack";

            return new StratSummary
            {
                Id = s.Id,
                Name = s.Name,
                VideoUrl = s.VideoUrl,
                Description = s.Description,
                Map = mapById.GetValueOrDefault(s.MapId) ?? new Map(s.MapId, $"Map {s.MapId}"),
                Side = side,
                Categories = cats ?? [],
                Operators = ops ?? []
            };
        }).ToList();
    }

    /// <summary>
    /// Deletes a strategy from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the strategy to delete.</param>
    public async Task DeleteStrat(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "DELETE FROM strats WHERE id=@id";
        await db.ExecuteAsync(sql, new { id = id });
    }

    /// <summary>
    /// Updates an existing strategy in the database.
    /// </summary>
    /// <param name="strat">The strategy object with updated data.</param>
    /// <returns>True if the strategy was found and updated; otherwise, false.</returns>
    public async Task<bool> UpdateStrat(Strat strat)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = @"UPDATE strats
                        SET name = @name,
                            video_url = @videoUrl,
                            map_id = @mapId,
                            description = @description
                        WHERE id = @id";

        int rows = await db.ExecuteAsync(sql, new
        {
            id = strat.Id,
            name = strat.Name,
            videoUrl = strat.VideoUrl,
            mapId = strat.MapId,
            description = strat.Description
        });

        return rows > 0;
    }

    /// <summary>
    /// Retrieves all maps from the database.
    /// </summary>
    /// <returns>A list of all <see cref="Map"/> objects.</returns>
    public async Task<List<Map>> GetAllMaps()
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id, name FROM maps ORDER BY name";
        return (await db.QueryAsync<Map>(sql)).ToList();
    }

    /// <summary>
    /// Retrieves a map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map.</param>
    /// <returns>A <see cref="Map"/> object if found; otherwise, null.</returns>
    public async Task<Map?> GetMapById(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT * FROM maps WHERE id=@id";
        return await db.QuerySingleOrDefaultAsync<Map>(sql, new { id = id });
    }

    /// <summary>
    /// Retrieves all existing categories from the database.
    /// </summary>
    /// <returns>A list of <see cref="Category"/> objects.</returns>
    public async Task<List<Category>> GetAllCategories()
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id, name, side FROM categories";
        return (await db.QueryAsync<Category>(sql)).ToList();
    }

    /// <summary>
    /// Retrieves a specific category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category.</param>
    /// <returns>A <see cref="Category"/> object if found; otherwise, null.</returns>
    public async Task<Category?> GetCategoryById(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id, name, side FROM categories WHERE id=@id";
        return await db.QuerySingleOrDefaultAsync<Category>(sql, new { id = id });
    }

    /// <summary>
    /// Retrieves multiple categories by their IDs.
    /// </summary>
    /// <param name="ids">The category IDs to retrieve.</param>
    /// <returns>A list of matching <see cref="Category"/> objects.</returns>
    public async Task<List<Category>> GetCategoriesByIds(List<int> ids)
    {
        if (ids.Count == 0)
            return [];

        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id, name, side FROM categories WHERE id IN @ids";
        return (await db.QueryAsync<Category>(sql, new { ids })).ToList();
    }

    /// <summary>
    /// Retrieves the ID of a category by its unique name.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <returns>The category ID if found; otherwise, null.</returns>
    public async Task<int?> GetCategoryIdByName(string categoryName)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id FROM categories WHERE name=@name";
        return await db.QuerySingleOrDefaultAsync<int?>(sql, new { name = categoryName });
    }

    /// <summary>
    /// Deletes a category from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    public async Task DeleteCategory(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "DELETE FROM categories WHERE id=@id";
        await db.ExecuteAsync(sql, new { id = id });
    }

    /// <summary>
    /// Adds a new category to the database and updates the object with its generated database ID.
    /// </summary>
    /// <param name="category">The category object containing the data to insert.</param>
    public async Task<int> AddCategory(Category category)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "INSERT INTO categories (name, side) " +
                     "VALUES (@name, @side); " +
                     "SELECT last_insert_rowid(); ";
        int newId = await db.QuerySingleAsync<int>(sql, new
        {
            name = category.Name,
            side = category.Side.ToString()
        });

        category.SetId(newId);
        return newId;
    }

    /// <summary>
    /// Creates a connection between a strategy and a category in the many-to-many table.
    /// </summary>
    /// <param name="stratId">The ID of the strategy.</param>
    /// <param name="categoryId">The ID of the category.</param>
    public async Task AssignStratToCategory(int stratId, int categoryId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "INSERT INTO strat_categories (strat_id, category_id) " +
                     "VALUES (@stratId, @categoryId)";
        await db.ExecuteAsync(sql, new { stratId = stratId, categoryId = categoryId });
    }

    /// <summary>
    /// Removes a category assignment from a strategy.
    /// </summary>
    /// <param name="stratId">The ID of the strategy.</param>
    /// <param name="categoryId">The ID of the category.</param>
    /// <returns>True if a relation was deleted; otherwise, false.</returns>
    public async Task<bool> RemoveCategoryFromStrat(int stratId, int categoryId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "DELETE FROM strat_categories WHERE strat_id = @stratId AND category_id = @categoryId";
        int rows = await db.ExecuteAsync(sql, new { stratId, categoryId });
        return rows > 0;
    }

    /// <summary>
    /// Retrieves all strategies associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category.</param>
    /// <returns>A list of <see cref="Strat"/> objects belonging to the category.</returns>
    public async Task<List<Strat>?> GetStratsByCategory(int categoryId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT s.id, s.name, s.video_url AS videoUrl, s.map_id AS mapId, s.description " +
                     "FROM strats s " +
                     "JOIN strat_categories sc ON s.id = sc.strat_id " +
                     "WHERE sc.category_id = @categoryId";
        return (await db.QueryAsync<Strat>(sql, new { categoryId = categoryId })).ToList();
    }

    /// <summary>
    /// Retrieves the name of a category by its ID.
    /// </summary>
    /// <param name="categoryId">The ID of the category.</param>
    /// <returns>The name of the category as a string if found; otherwise, null.</returns>
    public async Task<string?> GetCategoryNameById(int categoryId)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT name FROM categories WHERE id=@id";
        return await db.QuerySingleOrDefaultAsync<string>(sql, new { id = categoryId });
    }

    /// <summary>
    /// Adds a category proposal to the pending submissions table.
    /// </summary>
    /// <param name="submission">The category submission to store for review.</param>
    /// <returns>The generated pending submission ID.</returns>
    public async Task<int> AddPendingCategorySubmission(CategorySubmission submission)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = """
                     INSERT INTO pending_category_submissions (name, side)
                     VALUES (@name, @side);
                     SELECT last_insert_rowid();
                     """;

        return await db.QuerySingleAsync<int>(sql, new
        {
            name = submission.Name,
            side = submission.Side.ToString()
        });
    }

    /// <summary>
    /// Retrieves all category submissions waiting for admin review.
    /// </summary>
    /// <returns>A list of pending category submissions.</returns>
    public async Task<List<CategorySubmission>> GetPendingCategorySubmissions()
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = """
                     SELECT id, name, side, submitted_at AS SubmittedAt
                     FROM pending_category_submissions
                     ORDER BY submitted_at ASC, id ASC
                     """;

        return (await db.QueryAsync<CategorySubmission>(sql)).ToList();
    }

    /// <summary>
    /// Retrieves a single category submission by ID.
    /// </summary>
    /// <param name="id">The pending category submission ID.</param>
    /// <returns>The pending submission if found; otherwise, null.</returns>
    public async Task<CategorySubmission?> GetPendingCategorySubmissionById(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = """
                     SELECT id, name, side, submitted_at AS SubmittedAt
                     FROM pending_category_submissions
                     WHERE id=@id
                     """;

        return await db.QuerySingleOrDefaultAsync<CategorySubmission>(sql, new { id });
    }

    /// <summary>
    /// Approves a pending category submission and moves it into the categories table.
    /// </summary>
    /// <param name="id">The pending category submission ID.</param>
    /// <returns>The generated category ID if approved; otherwise, null.</returns>
    public async Task<int?> ApprovePendingCategorySubmission(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        await using var transaction = await db.BeginTransactionAsync();

        CategorySubmission? submission = await db.QuerySingleOrDefaultAsync<CategorySubmission>(
            """
            SELECT id, name, side, submitted_at AS SubmittedAt
            FROM pending_category_submissions
            WHERE id=@id
            """,
            new { id },
            transaction);

        if (submission == null)
        {
            await transaction.RollbackAsync();
            return null;
        }

        int categoryId = await db.QuerySingleAsync<int>(
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

        await db.ExecuteAsync(
            "DELETE FROM pending_category_submissions WHERE id=@id",
            new { id },
            transaction);

        await transaction.CommitAsync();
        return categoryId;
    }

    /// <summary>
    /// Deletes a category submission without approving it.
    /// </summary>
    /// <param name="id">The pending category submission ID.</param>
    /// <returns>True if a row was deleted; otherwise, false.</returns>
    public async Task<bool> DeletePendingCategorySubmission(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        int rows = await db.ExecuteAsync("DELETE FROM pending_category_submissions WHERE id=@id", new { id });
        return rows > 0;
    }

    /// <summary>
    /// Adds a strategy proposal and its proposed category/operator assignments.
    /// </summary>
    /// <param name="submission">The strategy submission to store for review.</param>
    /// <returns>The generated pending submission ID.</returns>
    public async Task<int> AddPendingStratSubmission(StratSubmission submission)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        await using var transaction = await db.BeginTransactionAsync();

        int submissionId = await db.QuerySingleAsync<int>(
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
            await db.ExecuteAsync(
                """
                INSERT INTO pending_strat_submission_categories (submission_id, category_id)
                VALUES (@submissionId, @categoryId)
                """,
                new { submissionId, categoryId },
                transaction);
        }

        foreach (int operatorId in submission.OperatorIds.Distinct())
        {
            await db.ExecuteAsync(
                """
                INSERT INTO pending_strat_submission_operators (submission_id, operator_id)
                VALUES (@submissionId, @operatorId)
                """,
                new { submissionId, operatorId },
                transaction);
        }

        await transaction.CommitAsync();
        return submissionId;
    }

    /// <summary>
    /// Retrieves all strategy submissions waiting for admin review.
    /// </summary>
    /// <returns>A list of pending strategy submissions.</returns>
    public async Task<List<StratSubmission>> GetPendingStratSubmissions()
    {
        await using SqliteConnection db = await GetConnectionAsync();
        string sql = """
                     SELECT id, name, video_url AS VideoUrl, map_id AS MapId, description, submitted_at AS SubmittedAt
                     FROM pending_strat_submissions
                     ORDER BY submitted_at ASC, id ASC
                     """;

        List<StratSubmission> submissions = (await db.QueryAsync<StratSubmission>(sql)).ToList();
        foreach (StratSubmission submission in submissions)
        {
            await LoadPendingStratSubmissionRelations(db, submission);
        }

        return submissions;
    }

    /// <summary>
    /// Retrieves a single strategy submission by ID.
    /// </summary>
    /// <param name="id">The pending strategy submission ID.</param>
    /// <returns>The pending submission if found; otherwise, null.</returns>
    public async Task<StratSubmission?> GetPendingStratSubmissionById(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        StratSubmission? submission = await db.QuerySingleOrDefaultAsync<StratSubmission>(
            """
            SELECT id, name, video_url AS VideoUrl, map_id AS MapId, description, submitted_at AS SubmittedAt
            FROM pending_strat_submissions
            WHERE id=@id
            """,
            new { id });

        if (submission == null) return null;

        await LoadPendingStratSubmissionRelations(db, submission);
        return submission;
    }

    /// <summary>
    /// Approves a pending strategy submission and moves it into the public strategy tables.
    /// </summary>
    /// <param name="id">The pending strategy submission ID.</param>
    /// <returns>The generated strategy ID if approved; otherwise, null.</returns>
    public async Task<int?> ApprovePendingStratSubmission(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        await using var transaction = await db.BeginTransactionAsync();

        StratSubmission? submission = await db.QuerySingleOrDefaultAsync<StratSubmission>(
            """
            SELECT id, name, video_url AS VideoUrl, map_id AS MapId, description, submitted_at AS SubmittedAt
            FROM pending_strat_submissions
            WHERE id=@id
            """,
            new { id },
            transaction);

        if (submission == null)
        {
            await transaction.RollbackAsync();
            return null;
        }

        int stratId = await db.QuerySingleAsync<int>(
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

        IEnumerable<int> categoryIds = await db.QueryAsync<int>(
            """
            SELECT category_id
            FROM pending_strat_submission_categories
            WHERE submission_id=@id
            """,
            new { id },
            transaction);

        foreach (int categoryId in categoryIds)
        {
            await db.ExecuteAsync(
                """
                INSERT INTO strat_categories (strat_id, category_id)
                VALUES (@stratId, @categoryId)
                """,
                new { stratId, categoryId },
                transaction);
        }

        IEnumerable<int> operatorIds = await db.QueryAsync<int>(
            """
            SELECT operator_id
            FROM pending_strat_submission_operators
            WHERE submission_id=@id
            """,
            new { id },
            transaction);

        foreach (int operatorId in operatorIds)
        {
            await db.ExecuteAsync(
                """
                INSERT INTO strat_operators (strat_id, operator_id)
                VALUES (@stratId, @operatorId)
                """,
                new { stratId, operatorId },
                transaction);
        }

        await db.ExecuteAsync("DELETE FROM pending_strat_submissions WHERE id=@id", new { id }, transaction);

        await transaction.CommitAsync();
        return stratId;
    }

    /// <summary>
    /// Deletes a strategy submission without approving it.
    /// </summary>
    /// <param name="id">The pending strategy submission ID.</param>
    /// <returns>True if a row was deleted; otherwise, false.</returns>
    public async Task<bool> DeletePendingStratSubmission(int id)
    {
        await using SqliteConnection db = await GetConnectionAsync();
        int rows = await db.ExecuteAsync("DELETE FROM pending_strat_submissions WHERE id=@id", new { id });
        return rows > 0;
    }

    private static async Task LoadPendingStratSubmissionRelations(SqliteConnection db, StratSubmission submission)
    {
        submission.CategoryIds = (await db.QueryAsync<int>(
            """
            SELECT category_id
            FROM pending_strat_submission_categories
            WHERE submission_id=@submissionId
            ORDER BY category_id
            """,
            new { submissionId = submission.Id })).ToList();

        submission.OperatorIds = (await db.QueryAsync<int>(
            """
            SELECT operator_id
            FROM pending_strat_submission_operators
            WHERE submission_id=@submissionId
            ORDER BY operator_id
            """,
            new { submissionId = submission.Id })).ToList();
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
    public async Task<bool> AssignOperatorToStrat(int stratId, int operatorId)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = """
                     INSERT INTO strat_operators (strat_id, operator_id)
                     VALUES (@stratId, @operatorId)
                     """;

        try
        {
            int rows = await db.ExecuteAsync(sql, new
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
            // - duplicate (if a UNIQUE constraint exists)
            return false;
        }
    }

    /// <summary>
    /// Checks whether a specific operator is assigned to a specific strategy.
    /// </summary>
    /// <param name="stratId">The ID of the strategy.</param>
    /// <param name="operatorId">The ID of the operator.</param>
    /// <returns>True if the operator is assigned to the strategy; otherwise, false.</returns>
    public async Task<bool> IsOperatorAssignedToStrat(int stratId, int operatorId)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = """
                     SELECT 1
                     FROM strat_operators
                     WHERE strat_id=@stratId AND operator_id=@operatorId
                     LIMIT 1
                     """;

        return await db.QuerySingleOrDefaultAsync<int?>(sql, new { stratId, operatorId }) != null;
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
    public async Task<bool> RemoveOperatorFromStrat(int stratId, int operatorId)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = """
                     DELETE FROM strat_operators
                     WHERE strat_id=@stratId
                       AND operator_id=@operatorId
                     """;

        int rowsAffected = await db.ExecuteAsync(sql, new
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
    public async Task<Operator?> GetOperatorById(int operatorId)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = """
                     SELECT id, name, side
                     FROM operators
                     WHERE id=@id
                     """;

        return await db.QuerySingleOrDefaultAsync<Operator>(
            sql,
            new { id = operatorId }
        );
    }

    /// <summary>
    /// Retrieves multiple operators by their IDs.
    /// </summary>
    /// <param name="ids">The operator IDs to retrieve.</param>
    /// <returns>A list of matching <see cref="Operator"/> objects.</returns>
    public async Task<List<Operator>> GetOperatorsByIds(List<int> ids)
    {
        if (ids.Count == 0)
            return [];

        await using SqliteConnection db = await GetConnectionAsync();
        string sql = "SELECT id, name, side FROM operators WHERE id IN @ids";
        return (await db.QueryAsync<Operator>(sql, new { ids })).ToList();
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
    public async Task<int?> GetOperatorIdByName(string operatorName)
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = """
                     SELECT id
                     FROM operators
                     WHERE name=@operatorName
                     """;

        return await db.QuerySingleOrDefaultAsync<int?>(
            sql,
            new { operatorName = operatorName }
        );
    }

    /// <summary>
    /// Retrieves all operators from the database.
    /// </summary>
    /// <returns>A list of all <see cref="Operator"/> objects.</returns>
    public async Task<List<Operator>> GetAllOperators()
    {
        await using SqliteConnection db = await GetConnectionAsync();

        string sql = """
                     SELECT id, name, side
                     FROM operators
                     """;
        return (await db.QueryAsync<Operator>(sql)).ToList();
    }
}
