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
    private readonly MapService _mapService;

    public MapsController(MapService mapService)
    {
        _mapService = mapService;
    }

    /// <summary>
    /// Returns all maps in the system.
    /// </summary>
    /// <returns>A list of all maps.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Map>>))]
    public async Task<IActionResult> GetAll()
    {
        var maps = await _mapService.GetAllMaps();
        return Ok(ApiResponse.Success(maps));
    }

    /// <summary>
    /// Returns a map by ID, including all of its bombsites.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>The map data with its bombsites if found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Map>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetById(int id)
    {
        Map? map = await _mapService.GetMapWithBombsites(id);
        if (map == null)
            return NotFound(ApiResponse.NotFound($"Map by id {id} not found"));

        return Ok(ApiResponse.Success(map));
    }
}
