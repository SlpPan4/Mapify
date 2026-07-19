using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

/// <summary>
/// API for managing operators and their assignment to strategies.
/// </summary>
[ApiController]
[Route("api/operators")]
[Produces("application/json")]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Operator>>))]
    public IActionResult GetAll()
    {
        var operators = _service.GetAllOperators();
        return Ok(ApiResponse.Success(operators));
    }

    /// <summary>
    /// Returns operator by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Operator>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public IActionResult GetById(int id)
    {
        var op = _service.GetOperatorById(id);

        if (op == null)
            return NotFound(ApiResponse.NotFound("Operator not found"));

        return Ok(ApiResponse.Success(op));
    }

    /// <summary>
    /// Returns operator ID by name.
    /// </summary>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public IActionResult GetIdByName(string name)
    {
        var id = _service.GetOperatorIdByName(name);

        if (id == null)
            return NotFound(ApiResponse.NotFound("Operator not found"));

        return Ok(ApiResponse.Success(new { id }));
    }

    /// <summary>
    /// Assign operator to strategy.
    /// </summary>
    [HttpPost("{operatorId:int}/assign/{stratId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult AssignToStrat(int operatorId, int stratId)
    {
        var result = _service.AssignOperatorToStrat(stratId, operatorId);

        if (!result)
            return BadRequest(ApiResponse.BadRequest("Could not assign operator to strategy"));

        return Ok(ApiResponse.SuccessMessage("Operator assigned successfully"));
    }

    /// <summary>
    /// Remove operator from strategy.
    /// </summary>
    [HttpDelete("{operatorId:int}/remove/{stratId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public IActionResult RemoveFromStrat(int operatorId, int stratId)
    {
        var result = _service.RemoveOperatorFromStrat(stratId, operatorId);

        if (!result)
            return NotFound(ApiResponse.NotFound("Relation not found"));

        return Ok(ApiResponse.SuccessMessage("Operator removed from strategy"));
    }
}
