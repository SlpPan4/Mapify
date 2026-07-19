using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

/// <summary>
/// API for public strategy/category submissions and admin review.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    /// <summary>
    /// Submits a new strategy for admin approval.
    /// </summary>
    [HttpPost("strats")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> SubmitStrat([FromBody] StratSubmissionRequest request)
    {
        try
        {
            InputValidator.ValidateStratSubmissionRequest(request);
            int submissionId = await _submissionService.SubmitStrat(request);
            return Ok(ApiResponse.Success(new { submissionId }, "Strategy submitted for approval"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse.NotFound(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error submitting strategy"));
        }
    }

    /// <summary>
    /// Submits a new category for admin approval.
    /// </summary>
    [HttpPost("categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> SubmitCategory([FromBody] CategorySubmissionRequest request)
    {
        try
        {
            InputValidator.ValidateCategorySubmissionRequest(request);
            int submissionId = await _submissionService.SubmitCategory(request);
            return Ok(ApiResponse.Success(new { submissionId }, "Category submitted for approval"));
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
            return BadRequest(ApiResponse.BadRequest("Error submitting category"));
        }
    }

    /// <summary>
    /// Returns all pending strategy submissions.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpGet("admin/strats")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<StratSubmission>>))]
    public async Task<IActionResult> GetPendingStrats()
    {
        return Ok(ApiResponse.Success(await _submissionService.GetPendingStratSubmissions()));
    }

    /// <summary>
    /// Returns a pending strategy submission by ID.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpGet("admin/strats/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<StratSubmission>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetPendingStrat(int id)
    {
        StratSubmission? submission = await _submissionService.GetPendingStratSubmission(id);
        if (submission == null)
            return NotFound(ApiResponse.NotFound($"Strategy submission by ID {id} was not found"));

        return Ok(ApiResponse.Success(submission));
    }

    /// <summary>
    /// Approves a pending strategy submission.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpPost("admin/strats/{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> ApproveStrat(int id)
    {
        try
        {
            int? stratId = await _submissionService.ApproveStratSubmission(id);
            if (stratId == null)
                return NotFound(ApiResponse.NotFound($"Strategy submission by ID {id} was not found"));

            return Ok(ApiResponse.Success(new { stratId }, "Strategy submission approved"));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error approving strategy submission"));
        }
    }

    /// <summary>
    /// Rejects a pending strategy submission.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpDelete("admin/strats/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> RejectStrat(int id)
    {
        if (!await _submissionService.RejectStratSubmission(id))
            return NotFound(ApiResponse.NotFound($"Strategy submission by ID {id} was not found"));

        return Ok(ApiResponse.SuccessMessage("Strategy submission rejected"));
    }

    /// <summary>
    /// Returns all pending category submissions.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpGet("admin/categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<CategorySubmission>>))]
    public async Task<IActionResult> GetPendingCategories()
    {
        return Ok(ApiResponse.Success(await _submissionService.GetPendingCategorySubmissions()));
    }

    /// <summary>
    /// Returns a pending category submission by ID.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpGet("admin/categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CategorySubmission>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetPendingCategory(int id)
    {
        CategorySubmission? submission = await _submissionService.GetPendingCategorySubmission(id);
        if (submission == null)
            return NotFound(ApiResponse.NotFound($"Category submission by ID {id} was not found"));

        return Ok(ApiResponse.Success(submission));
    }

    /// <summary>
    /// Approves a pending category submission.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpPost("admin/categories/{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> ApproveCategory(int id)
    {
        try
        {
            int? categoryId = await _submissionService.ApproveCategorySubmission(id);
            if (categoryId == null)
                return NotFound(ApiResponse.NotFound($"Category submission by ID {id} was not found"));

            return Ok(ApiResponse.Success(new { categoryId }, "Category submission approved"));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error approving category submission"));
        }
    }

    /// <summary>
    /// Rejects a pending category submission.
    /// Admin-only endpoint. Restrict this route at the hosting/auth layer before exposing it publicly.
    /// </summary>
    [HttpDelete("admin/categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> RejectCategory(int id)
    {
        if (!await _submissionService.RejectCategorySubmission(id))
            return NotFound(ApiResponse.NotFound($"Category submission by ID {id} was not found"));

        return Ok(ApiResponse.SuccessMessage("Category submission rejected"));
    }
}
