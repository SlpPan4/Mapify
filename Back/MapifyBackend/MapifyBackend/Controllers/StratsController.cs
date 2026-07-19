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
    /// Returns all strategies in the system.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Strat>>))]
    public async Task<IActionResult> GetAll()
    {
        var allStrats = await _stratService.GetAllStrats();
        return Ok(ApiResponse.Success(allStrats));
    }

    /// <summary>
    /// Returns a single strategy by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Strat>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetById(int id)
    {
        Strat? strat = await _stratService.GetStrat(id);
        if (strat == null)
        {
            return NotFound(ApiResponse.NotFound($"Strat by ID {id} was not found"));
        }

        return Ok(ApiResponse.Success(strat));
    }

    /// <summary>
    /// Returns a map by ID.
    /// </summary>
    [HttpGet("maps/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Map>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult GetMapById(int id)
    {
        try
        {
            Map? map = _db.GetMapById(id);
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
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Create([FromBody] StratRequest request)
    {
        try
        {
            InputValidator.ValidateStratRequest(request);
            await _stratService.CreateStrat(request.Name, request.VideoUrl, request.MapName);
            return Ok(ApiResponse.SuccessMessage("Strategy added!"));
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
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public IActionResult DeleteStrat(int id)
    {
        if (!_stratService.DeleteStrat(id))
            return NotFound(ApiResponse.NotFound($"Strat by ID {id} was not found"));

        return Ok(ApiResponse.SuccessMessage("Strategy deleted!"));
    }

    /// <summary>
    /// Assigns a strategy to a category.
    /// </summary>
    [HttpPost("assign/strat/{stratId}/category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult AssignStratToCategory(int stratId, int categoryId)
    {
        try
        {
            _stratService.AssignStratToCategory(stratId, categoryId);
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
    /// Returns a map ID by its name.
    /// </summary>
    [HttpGet("maps/byname/{mapName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult GetMapIdByName(string mapName)
    {
        var normalizedMapName = Capitalize(mapName);
        try
        {
            int? mapId = _stratService.GetMapIdByName(normalizedMapName);
            return Ok(ApiResponse.Success(mapId!.Value));
        }
        catch (ArgumentException)
        {
            return NotFound(ApiResponse.NotFound($"No maps found with such name {normalizedMapName}"));
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }
}
