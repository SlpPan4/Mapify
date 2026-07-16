namespace MapifyBackend.database_files;

/// <summary>
/// Service layer for managing strategies (strats) and their relations.
/// Acts as a mediator between the application logic and the database service.
/// </summary>
public class StratService
{
    private readonly DatabaseService _dbService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StratService"/> class.
    /// </summary>
    /// <param name="databaseService">The database service used for data persistence.</param>
    public StratService(DatabaseService databaseService)
    {
        _dbService = databaseService;
    }
    
    /// <summary>
    /// Creates a new strategy and saves it to the database.
    /// </summary>
    /// <param name="name">The name of the strategy.</param>
    /// <param name="videoUrl">The URL of the strategy's video guide.</param>
    /// <param name="mapName">The name of the map this strategy belongs to.</param>
    /// <throws cref="ArgumentException">Thrown when the specified map name does not exist in the database.</throws>
    public async Task<int> CreateStrat(string name, string videoUrl, string mapName)
    {
        int? mapId = GetMapIdByName(mapName);

        if (mapId == null)
            throw new ArgumentException($"Map '{mapName}' does not exist");

        Strat strat = new Strat(name, videoUrl, mapId.Value);
        return await _dbService.AddStrat(strat);
    }

    /// <summary>
    /// Retrieves the ID of a map by its name.
    /// </summary>
    /// <param name="mapName">The name of the map to search for.</param>
    /// <returns>The map ID if found; otherwise, null.</returns>
    public int? GetMapIdByName(string mapName)
    {
        return _dbService.GetMapIdByName(mapName);
    }

    /// <summary>
    /// Retrieves a single strategy by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the strategy.</param>
    /// <returns>The <see cref="Strat"/> object if found; otherwise, null.</returns>
    public async Task<Strat?> GetStrat(int id)
    {
        return await _dbService.GetStratById(id);
    }

    /// <summary>
    /// Retrieves all strategies available in the database.
    /// </summary>
    /// <returns>A list of all <see cref="Strat"/> objects.</returns>
    public async Task<List<Strat>> GetAllStrats()
    {
        return await _dbService.GetAllStrats();
    }

    /// <summary>
    /// Deletes a strategy by its ID if it exists.
    /// </summary>
    /// <param name="id">The ID of the strategy to delete.</param>
    /// <returns>True if the strategy was found and successfully deleted; otherwise, false.</returns>
    public bool DeleteStrat(int id)
    {
        var strat = GetStrat(id);
        if (strat == null) return false;

        _dbService.DeleteStrat(id);
        return true;
    }

    /// <summary>
    /// Links an existing strategy to an existing category.
    /// </summary>
    /// <param name="stratId">The ID of the strategy.</param>
    /// <param name="categoryId">The ID of the category.</param>
    /// <throws cref="ArgumentException">Thrown when either the strategy ID or the category ID is invalid.</throws>
    public void AssignStratToCategory(int stratId, int categoryId)
    {
        if (_dbService.GetStratById(stratId) == null)
        {
            throw new ArgumentException($"No strat by id {stratId}");
        }

        if (_dbService.GetCategoryById(categoryId) == null)
        {
            throw new ArgumentException($"No category by id {categoryId}");
        }
        
        _dbService.AssignStratToCategory(stratId, categoryId);
    }

    /// <summary>
    /// Retrieves all strategies that belong to a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category.</param>
    /// <returns>A list of <see cref="Strat"/> objects if the category exists; otherwise, null.</returns>
    public async Task<List<Strat>?> GetStratsByCategory(int categoryId)
    {
        if (_dbService.GetCategoryById(categoryId) == null)
        {
            return null;
        }
        return await _dbService.GetStratsByCategory(categoryId);
    }

    public async Task<List<Strat>?> GetStratsByMapId(int mapId)
    {
        return await _dbService.StratsByMapId(mapId);
    }

    public async Task<List<Strat>?> GetStratsByOperatorId(int operatorId)
    {
        return await _dbService.StratsByOperator(operatorId);
    }
}