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
    /// <returns>A list of all operators.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Operator>>))]
    public async Task<IActionResult> GetAll()
    {
        var operators = await _service.GetAllOperators();
        return Ok(ApiResponse.Success(operators));
    }

    /// <summary>
    /// Returns an operator by ID.
    /// </summary>
    /// <param name="id">The operator ID.</param>
    /// <returns>The operator data if found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Operator>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetById(int id)
    {
        var op = await _service.GetOperatorById(id);

        if (op == null)
            return NotFound(ApiResponse.NotFound("Operator not found"));

        return Ok(ApiResponse.Success(op));
    }

    /// <summary>
    /// Returns an operator ID by its exact name.
    /// </summary>
    /// <param name="name">The operator name.</param>
    /// <returns>An object containing the operator ID.</returns>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetIdByName(string name)
    {
        var id = await _service.GetOperatorIdByName(name);

        if (id == null)
            return NotFound(ApiResponse.NotFound("Operator not found"));

        return Ok(ApiResponse.Success(new { id }));
    }

    /// <summary>
    /// Assigns an operator to a strategy.
    /// </summary>
    /// <param name="operatorId">The operator ID.</param>
    /// <param name="stratId">The strategy ID.</param>
    /// <returns>A success message if the assignment succeeded.</returns>
    [HttpPost("{operatorId:int}/assign/{stratId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> AssignToStrat(int operatorId, int stratId)
    {
        var result = await _service.AssignOperatorToStrat(stratId, operatorId);

        if (!result)
            return BadRequest(ApiResponse.BadRequest("Could not assign operator to strategy"));

        return Ok(ApiResponse.SuccessMessage("Operator assigned successfully"));
    }

    /// <summary>
    /// Removes an operator from a strategy.
    /// </summary>
    /// <param name="operatorId">The operator ID.</param>
    /// <param name="stratId">The strategy ID.</param>
    /// <returns>A success message if the relation was removed.</returns>
    [HttpDelete("{operatorId:int}/remove/{stratId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> RemoveFromStrat(int operatorId, int stratId)
    {
        var result = await _service.RemoveOperatorFromStrat(stratId, operatorId);

        if (!result)
            return NotFound(ApiResponse.NotFound("Relation not found"));

        return Ok(ApiResponse.SuccessMessage("Operator removed from strategy"));
    }
}
