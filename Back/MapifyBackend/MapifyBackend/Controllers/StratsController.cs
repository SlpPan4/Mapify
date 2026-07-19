using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MapifyBackend.Utility.DataNormalizingHelpers.StringHelper;

namespace MapifyBackend.Controllers;

/// <summary>
/// API for managing Rainbow Six Siege strategies, maps, and their assignments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StratsController : ControllerBase
{
    private readonly DatabaseService _db;
    private readonly StratService _stratService;

    public StratsController(DatabaseService db, StratService stratService)
    {
        _db = db;
        _stratService = stratService;
    }

    /// <summary>
    /// Returns all strategies in the system, optionally filtered by name, map, category, or operator.
    /// </summary>
    /// <param name="name">Optional case-insensitive substring filter on strategy name.</param>
    /// <param name="mapId">Optional filter by map ID.</param>
    /// <param name="categoryId">Optional filter by assigned category ID.</param>
    /// <param name="operatorId">Optional filter by assigned operator ID.</param>
    /// <returns>A list of strategies matching the provided filters.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Strat>>))]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] int? mapId,
        [FromQuery] int? categoryId,
        [FromQuery] int? operatorId)
    {
        var strats = await _stratService.GetStratsFiltered(name, mapId, categoryId, operatorId);
        return Ok(ApiResponse.Success(strats));
    }

    /// <summary>
    /// Returns a single strategy by ID, including its map, categories, and operators.
    /// </summary>
    /// <param name="id">The strategy ID.</param>
    /// <returns>The detailed strategy data if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<StratDetail>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetById(int id)
    {
        StratDetail? strat = await _stratService.GetStratDetail(id);
        if (strat == null)
        {
            return NotFound(ApiResponse.NotFound($"Strat by ID {id} was not found"));
        }

        return Ok(ApiResponse.Success(strat));
    }

    /// <summary>
    /// Returns a map by ID.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>The map data if found.</returns>
    [HttpGet("maps/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Map>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetMapById(int id)
    {
        try
        {
            Map? map = await _db.GetMapById(id);
            if (map == null)
                return NotFound(ApiResponse.NotFound($"Map by id {id} not found"));

            return Ok(ApiResponse.Success(map));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Creates and posts a new strategy.
    /// </summary>
    /// <param name="request">The strategy creation request.</param>
    /// <returns>The ID of the newly created strategy with a Location header.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Create([FromBody] StratRequest request)
    {
        try
        {
            InputValidator.ValidateStratRequest(request);
            int stratId = await _stratService.CreateStrat(request.Name, request.VideoUrl, request.MapName);
            return Created($"/api/strats/{stratId}", ApiResponse.Created(new { stratId }, "Strategy added!"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (ArgumentException)
        {
            return NotFound(ApiResponse.NotFound($"No map by the name {request.MapName}"));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error creating strategy"));
        }
    }

    /// <summary>
    /// Deletes a strategy by ID.
    /// </summary>
    /// <param name="id">The strategy ID.</param>
    /// <returns>A success message if the strategy was deleted.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> DeleteStrat(int id)
    {
        if (!await _stratService.DeleteStrat(id))
            return NotFound(ApiResponse.NotFound($"Strat by ID {id} was not found"));

        return Ok(ApiResponse.SuccessMessage("Strategy deleted!"));
    }

    /// <summary>
    /// Fully replaces an existing strategy.
    /// </summary>
    /// <param name="id">The strategy ID.</param>
    /// <param name="request">The full update request.</param>
    /// <returns>A success message if the strategy was updated.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateStrat(int id, [FromBody] StratUpdateRequest request)
    {
        try
        {
            InputValidator.ValidateStratUpdateRequest(request);
            bool updated = await _stratService.UpdateStrat(id, request);
            if (!updated)
                return NotFound(ApiResponse.NotFound($"Strat by ID {id} was not found"));

            return Ok(ApiResponse.SuccessMessage("Strategy updated!"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (ArgumentException)
        {
            return NotFound(ApiResponse.NotFound($"No map by the name {request.MapName}"));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error updating strategy"));
        }
    }

    /// <summary>
    /// Partially updates an existing strategy. Only provided fields are changed.
    /// </summary>
    /// <param name="id">The strategy ID.</param>
    /// <param name="request">The partial update request.</param>
    /// <returns>A success message if the strategy was updated.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> PatchStrat(int id, [FromBody] StratPatchRequest request)
    {
        try
        {
            InputValidator.ValidateStratPatchRequest(request);
            bool updated = await _stratService.PatchStrat(id, request);
            if (!updated)
                return NotFound(ApiResponse.NotFound($"Strat by ID {id} was not found"));

            return Ok(ApiResponse.SuccessMessage("Strategy updated!"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (ArgumentException)
        {
            return NotFound(ApiResponse.NotFound($"No map by the name {request.MapName}"));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error updating strategy"));
        }
    }

    /// <summary>
    /// Assigns a strategy to a category.
    /// </summary>
    /// <param name="stratId">The strategy ID.</param>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>A success message if the assignment succeeded.</returns>
    [HttpPost("assign/strat/{stratId}/category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> AssignStratToCategory(int stratId, int categoryId)
    {
        try
        {
            await _stratService.AssignStratToCategory(stratId, categoryId);
            return Ok(ApiResponse.SuccessMessage("Strat assigned"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
    }

    /// <summary>
    /// Returns all strategies in a category.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <returns>A list of strategies in the category.</returns>
    [HttpGet("category/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Strat>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetStratsByCategory(int id)
    {
        var strats = await _stratService.GetStratsByCategory(id);
        if (strats == null || strats.Count == 0)
            return NotFound(ApiResponse.NotFound($"No strats found in category {id}"));

        return Ok(ApiResponse.Success(strats));
    }

    /// <summary>
    /// Returns all strategies for a map.
    /// </summary>
    /// <param name="id">The map ID.</param>
    /// <returns>A list of strategies for the map.</returns>
    [HttpGet("bymap/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Strat>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetStratsByMap(int id)
    {
        var strats = await _stratService.GetStratsByMapId(id);
        if (strats == null || strats.Count == 0)
            return NotFound(ApiResponse.NotFound($"No strats found in map {id}"));

        return Ok(ApiResponse.Success(strats));
    }

    /// <summary>
    /// Returns all strategies that use a specific operator.
    /// </summary>
    /// <param name="id">The operator ID.</param>
    /// <returns>A list of strategies using the operator.</returns>
    [HttpGet("byoperator/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Strat>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetStratsByOperator(int id)
    {
        var strats = await _stratService.GetStratsByOperatorId(id);
        if (strats == null || strats.Count == 0)
            return NotFound(ApiResponse.NotFound($"No strats found by operator {id}"));

        return Ok(ApiResponse.Success(strats));
    }

    /// <summary>
    /// Returns a map ID by its name. The match is case-insensitive.
    /// </summary>
    /// <param name="mapName">The map name.</param>
    /// <returns>The ID of the map if found.</returns>
    [HttpGet("maps/byname/{mapName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetMapIdByName(string mapName)
    {
        var normalizedMapName = Capitalize(mapName);
        try
        {
            int? mapId = await _stratService.GetMapIdByName(normalizedMapName);
            if (mapId == null)
                return NotFound(ApiResponse.NotFound($"No maps found with such name {normalizedMapName}"));

            return Ok(ApiResponse.Success(mapId.Value));
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }
}
