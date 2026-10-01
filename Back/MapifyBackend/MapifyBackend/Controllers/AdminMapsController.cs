using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

/// <summary>
/// Admin API for managing maps and their bombsites.
/// All endpoints require the X-Api-Key header (see ApiKeyAuthMiddleware).
/// </summary>
[ApiController]
[Route("api/admin/maps")]
[Produces("application/json")]
public class AdminMapsController : ControllerBase
{
    private readonly MapService _mapService;
    private readonly BombsiteService _bombsiteService;

    public AdminMapsController(MapService mapService, BombsiteService bombsiteService)
    {
        _mapService = mapService;
        _bombsiteService = bombsiteService;
    }

    /// <summary>
    /// Creates a new map.
    /// </summary>
    /// <param name="request">The map creation request.</param>
    /// <returns>The ID of the newly created map with a Location header.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateMap([FromBody] MapRequest request)
    {
        try
        {
            InputValidator.ValidateMapRequest(request);
            int mapId = await _mapService.AddMap(request.Name);
            return Created($"/api/maps/{mapId}", ApiResponse.Created(new { mapId }, "Map added"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error adding map"));
        }
    }

    /// <summary>
    /// Renames an existing map.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <param name="request">The map update request.</param>
    /// <returns>A success message if the map was updated.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateMap(int id, [FromBody] MapRequest request)
    {
        try
        {
            InputValidator.ValidateMapRequest(request);
            if (!await _mapService.UpdateMap(id, request.Name))
                return NotFound(ApiResponse.NotFound($"Map by id {id} was not found"));

            return Ok(ApiResponse.SuccessMessage("Map updated"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error updating map"));
        }
    }

    /// <summary>
    /// Deletes a map by ID. Deletion is blocked while strats or pending
    /// submissions still reference the map.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>A success message if the map was deleted.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> DeleteMap(int id)
    {
        try
        {
            if (!await _mapService.DeleteMap(id))
                return NotFound(ApiResponse.NotFound($"Map by id {id} was not found"));

            return Ok(ApiResponse.SuccessMessage("Map deleted"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error deleting map"));
        }
    }

    /// <summary>
    /// Adds a new bombsite to a map.
    /// </summary>
    /// <param name="mapId">The ID of the map the bombsite belongs to.</param>
    /// <param name="request">The bombsite creation request.</param>
    /// <returns>The ID of the newly created bombsite.</returns>
    [HttpPost("{mapId:int}/bombsites")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> AddBombsite(int mapId, [FromBody] BombsiteRequest request)
    {
        try
        {
            InputValidator.ValidateBombsiteRequest(request);
            int bombsiteId = await _bombsiteService.AddBombsite(mapId, request.Name);
            return Created($"/api/maps/{mapId}", ApiResponse.Created(new { bombsiteId }, "Bombsite added"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse.NotFound(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error adding bombsite"));
        }
    }

    /// <summary>
    /// Renames a bombsite of a map.
    /// </summary>
    /// <param name="mapId">The map ID.</param>
    /// <param name="id">The bombsite ID.</param>
    /// <param name="request">The bombsite update request.</param>
    /// <returns>A success message if the bombsite was updated.</returns>
    [HttpPut("{mapId:int}/bombsites/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateBombsite(int mapId, int id, [FromBody] BombsiteRequest request)
    {
        try
        {
            InputValidator.ValidateBombsiteRequest(request);

            Bombsite? bombsite = await _bombsiteService.GetBombsiteById(id);
            if (bombsite == null || bombsite.MapId != mapId)
                return NotFound(ApiResponse.NotFound($"Bombsite by id {id} was not found for map {mapId}"));

            await _bombsiteService.UpdateBombsite(id, request.Name);
            return Ok(ApiResponse.SuccessMessage("Bombsite updated"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error updating bombsite"));
        }
    }

    /// <summary>
    /// Deletes a bombsite from a map. Strats and pending submissions that
    /// reference it are unlinked.
    /// </summary>
    /// <param name="mapId">The map ID.</param>
    /// <param name="id">The bombsite ID.</param>
    /// <returns>A success message if the bombsite was deleted.</returns>
    [HttpDelete("{mapId:int}/bombsites/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> DeleteBombsite(int mapId, int id)
    {
        Bombsite? bombsite = await _bombsiteService.GetBombsiteById(id);
        if (bombsite == null || bombsite.MapId != mapId)
            return NotFound(ApiResponse.NotFound($"Bombsite by id {id} was not found for map {mapId}"));

        await _bombsiteService.DeleteBombsite(id);
        return Ok(ApiResponse.SuccessMessage("Bombsite deleted"));
    }
}
