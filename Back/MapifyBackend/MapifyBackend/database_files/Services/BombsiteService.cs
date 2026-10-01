namespace MapifyBackend.database_files;

/// <summary>
/// Service layer for managing bombsites.
/// </summary>
public class BombsiteService
{
    private readonly DatabaseService _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="BombsiteService"/> class.
    /// </summary>
    /// <param name="db">The database service used for data persistence.</param>
    public BombsiteService(DatabaseService db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a bombsite by its ID.
    /// </summary>
    /// <param name="id">The bombsite ID.</param>
    /// <returns>The bombsite if found; otherwise, null.</returns>
    public async Task<Bombsite?> GetBombsiteById(int id)
    {
        return await _db.GetBombsiteById(id);
    }

    /// <summary>
    /// Retrieves all bombsites that belong to a map.
    /// </summary>
    /// <param name="mapId">The map ID.</param>
    /// <returns>A list of bombsites for the map.</returns>
    public async Task<List<Bombsite>> GetBombsitesByMapId(int mapId)
    {
        return await _db.GetBombsitesByMapId(mapId);
    }

    /// <summary>
    /// Adds a new bombsite to a map.
    /// </summary>
    /// <param name="mapId">The ID of the map the bombsite belongs to.</param>
    /// <param name="name">The bombsite name.</param>
    /// <returns>The ID of the newly created bombsite.</returns>
    /// <exception cref="ArgumentException">Thrown when the map does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the map already has a bombsite with the same name.</exception>
    public async Task<int> AddBombsite(int mapId, string name)
    {
        if (await _db.GetMapById(mapId) == null)
            throw new ArgumentException($"Map by id {mapId} does not exist");

        string trimmed = name.Trim();
        List<Bombsite> existing = await _db.GetBombsitesByMapId(mapId);
        if (existing.Any(b => string.Equals(b.Name, trimmed, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Bombsite '{trimmed}' already exists for map {mapId}");

        return await _db.AddBombsite(mapId, trimmed);
    }

    /// <summary>
    /// Renames an existing bombsite.
    /// </summary>
    /// <param name="id">The bombsite ID.</param>
    /// <param name="name">The new bombsite name.</param>
    /// <returns>True if the bombsite was found and updated; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the map already has another bombsite with the same name.</exception>
    public async Task<bool> UpdateBombsite(int id, string name)
    {
        Bombsite? bombsite = await _db.GetBombsiteById(id);
        if (bombsite == null)
            return false;

        string trimmed = name.Trim();
        List<Bombsite> siblings = await _db.GetBombsitesByMapId(bombsite.MapId);
        if (siblings.Any(b => b.Id != id && string.Equals(b.Name, trimmed, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Bombsite '{trimmed}' already exists for map {bombsite.MapId}");

        return await _db.UpdateBombsite(id, trimmed);
    }

    /// <summary>
    /// Deletes a bombsite. Strats and pending submissions that reference it are unlinked.
    /// </summary>
    /// <param name="id">The bombsite ID.</param>
    /// <returns>True if the bombsite was found and deleted; otherwise, false.</returns>
    public async Task<bool> DeleteBombsite(int id)
    {
        return await _db.DeleteBombsite(id);
    }
}
