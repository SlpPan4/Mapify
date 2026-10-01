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

    /// <summary>
    /// Retrieves a map by its ID, including all of its bombsites.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>The map with its bombsites if found; otherwise, null.</returns>
    public async Task<Map?> GetMapWithBombsites(int id)
    {
        return await _db.GetMapWithBombsitesById(id);
    }

    /// <summary>
    /// Creates a new map.
    /// </summary>
    /// <param name="name">The name of the map.</param>
    /// <returns>The ID of the newly created map.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a map with the same name already exists.</exception>
    public async Task<int> AddMap(string name)
    {
        string trimmed = name.Trim();
        if (await _db.GetMapIdByName(trimmed) != null)
            throw new InvalidOperationException($"Map '{trimmed}' already exists");

        return await _db.AddMap(trimmed);
    }

    /// <summary>
    /// Renames an existing map.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <param name="name">The new map name.</param>
    /// <returns>True if the map was found and updated; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when another map with the same name already exists.</exception>
    public async Task<bool> UpdateMap(int id, string name)
    {
        if (await _db.GetMapById(id) == null)
            return false;

        string trimmed = name.Trim();
        int? existingId = await _db.GetMapIdByName(trimmed);
        if (existingId != null && existingId != id)
            throw new InvalidOperationException($"Map '{trimmed}' already exists");

        return await _db.UpdateMap(id, trimmed);
    }

    /// <summary>
    /// Deletes a map by its ID if it exists.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>True if the map was found and deleted; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when strats or pending submissions still reference the map.</exception>
    public async Task<bool> DeleteMap(int id)
    {
        if (await _db.GetMapById(id) == null)
            return false;

        await _db.DeleteMap(id);
        return true;
    }
}
