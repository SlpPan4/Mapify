using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MapifyBackend.Utility.DataNormalizingHelpers.StringHelper;

namespace MapifyBackend.Controllers;

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
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Strat>))]
    public async Task<IActionResult> GetAll()
    {
        var allStrats = await _stratService.GetAllStrats();
        return Ok(allStrats);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Strat))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        Strat? strat = await _stratService.GetStrat(id);
        if (strat == null)
        {
            return NotFound(new { message = $"Strat by ID {id} was not found" });
        }

        return Ok(strat);
    }

    [HttpGet("maps/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Map))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetMapById(int id)
    {
        try
        {
            Map? mapName = _db.GetMapById(id);
            if (mapName == null) return NotFound(new { message = $"Map by id {id} not found" });
            return Ok(mapName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] StratRequest request)
    {
        try
        {
            InputValidator.ValidateStratRequest(request);
            await _stratService.CreateStrat(request.Name, request.VideoUrl, request.MapName);
            return Ok(new { message = "Strategy added!" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException)
        {
            return NotFound(new { error = $"No map by the name {request.MapName}" });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error creating strategy" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteStrat(int id)
    {
        if (!_stratService.DeleteStrat(id)) return NotFound(new { message = $"Strat by ID {id} was not found" });
        return Ok(new { message = "Strategy deleted!" });
    }

    [HttpPost("assign/strat/{stratId}/category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AssignStratToCategory(int stratId, int categoryId)
    {
        try
        {
            _stratService.AssignStratToCategory(stratId, categoryId);
            return Ok(new { message = "Strat assigned" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("category/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Strat>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetStratsByCategory(int id)
    {
        var strats = _stratService.GetStratsByCategory(id);
        if (strats == null || strats.Count == 0) 
            return NotFound(new { message = $"No strats found in category {id}" });
        return Ok(strats);  
    }

    [HttpGet("maps/byname/{mapName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetMapIdByName(string mapName)
    {
        var normalizedMapName = Capitalize(mapName);
        try
        {
            return Ok(_stratService.GetMapIdByName(normalizedMapName));
        }
        catch (ArgumentException)
        {
            return NotFound(new { message = $"No maps found with such name {normalizedMapName}" });
        }
        catch (Exception e)
        {
            return BadRequest((new { error = e.Message }));
        }
    }
}