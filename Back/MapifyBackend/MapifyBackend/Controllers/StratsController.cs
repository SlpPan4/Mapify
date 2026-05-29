using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StratsController : ControllerBase
{
    private readonly DatabaseService _db;
    private readonly StratService _stratService;

    public StratsController(DatabaseService db, StratService stratService)
    {
        _db = db;
        _stratService = stratService;
    }
    
    //Get all strats
    [HttpGet]
    public IActionResult GetAll()
    {
        var allStrats = _db.GetAllStrats();
        return Ok(allStrats); //status 200 and JSON data
    }

    //Get 1 strat by ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        Strat? strat = _stratService.GetStratId(id);
        if (strat == null)
        {
            return NotFound(new { message = $"Strat by ID {id} was not found" });
        }

        return Ok(strat);
    }

    [HttpGet("maps/{id}")]
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
    
    //Create strat
    [HttpPost]
    public IActionResult Create([FromBody] StratRequest request)
    {
        try
        {
            InputValidator.ValidateStratRequest(request);
            _stratService.CreateStrat(request.Name, request.VideoUrl, request.MapName);
            return Ok(new { message = "Strategy added!" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error creating strategy" });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteStrat(int id)
    {
        if (!_stratService.DeleteStrat(id)) return NotFound(new { message = $"Strat by ID {id} was not found" });
        return Ok(new { message = "Strategy deleted!" });
    }

    [HttpPost("{stratId}/{categoryId}")]
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
    public IActionResult GetStratsByCategory(int id)
    {
        var strats = _stratService.GetStratsByCategory(id);
        if (strats == null || strats.Count == 0) 
            return NotFound(new { message = $"No strats found in category {id}" });
        return Ok(strats);  
    }
}

