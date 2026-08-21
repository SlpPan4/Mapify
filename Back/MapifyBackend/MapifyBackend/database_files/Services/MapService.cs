namespace MapifyBackend.database_files;

/// <summary>
/// Service layer for managing maps.
/// </summary>
public class MapService
{
    private readonly DatabaseService _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapService"/> class.
    /// </summary>
    /// <param name="db">The database service used for data persistence.</param>
    public MapService(DatabaseService db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all maps in the system.
    /// </summary>
    /// <returns>A list of all maps.</returns>
    public async Task<List<Map>> GetAllMaps()
    {
        return await _db.GetAllMaps();
    }

    /// <summary>
    /// Retrieves a map by its ID.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>The map if found; otherwise, null.</returns>
    public async Task<Map?> GetMapById(int id)
    {
        return await _db.GetMapById(id);
    }
}
