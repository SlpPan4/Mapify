using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

/// <summary>
/// API for managing Rainbow Six Siege maps.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MapsController : ControllerBase
{
    private readonly DatabaseService _db;

    public MapsController(DatabaseService db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all maps in the system.
    /// </summary>
    /// <returns>A list of all maps.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Map>>))]
    public async Task<IActionResult> GetAll()
    {
        var maps = await _db.GetAllMaps();
        return Ok(ApiResponse.Success(maps));
    }
}
