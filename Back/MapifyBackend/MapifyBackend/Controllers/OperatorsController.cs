using MapifyBackend.database_files;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

[ApiController]
[Route("api/operators")]
public class OperatorsController : ControllerBase
{
    private readonly OperatorService _service;

    public OperatorsController(OperatorService service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns all operators in the system.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var operators = _service.GetAllOperators();
        return Ok(operators);
    }

    /// <summary>
    /// Returns operator by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var op = _service.GetOperatorById(id);

        if (op == null)
            return NotFound(new { message = "Operator not found" });

        return Ok(op);
    }

    /// <summary>
    /// Returns operator ID by name.
    /// </summary>
    [HttpGet("by-name/{name}")]
    public IActionResult GetIdByName(string name)
    {
        var id = _service.GetOperatorIdByName(name);

        if (id == null)
            return NotFound(new { message = "Operator not found" });

        return Ok(new { id });
    }

    /// <summary>
    /// Assign operator to strategy.
    /// </summary>
    [HttpPost("{operatorId:int}/assign/{stratId:int}")]
    public IActionResult AssignToStrat(int operatorId, int stratId)
    {
        var result = _service.AssignOperatorToStrat(stratId, operatorId);

        if (!result)
            return BadRequest(new { message = "Could not assign operator to strategy" });

        return Ok(new { message = "Operator assigned successfully" });
    }

    /// <summary>
    /// Remove operator from strategy.
    /// </summary>
    [HttpDelete("{operatorId:int}/remove/{stratId:int}")]
    public IActionResult RemoveFromStrat(int operatorId, int stratId)
    {
        var result = _service.RemoveOperatorFromStrat(stratId, operatorId);

        if (!result)
            return NotFound(new { message = "Relation not found" });

        return Ok(new { message = "Operator removed from strategy" });
    }
}