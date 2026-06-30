using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.DTOs;
using MapifyBackend.Utility.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

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

    [HttpPost("strats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitStrat([FromBody] StratSubmissionRequest request)
    {
        try
        {
            InputValidator.ValidateStratSubmissionRequest(request);
            int submissionId = await _submissionService.SubmitStrat(request);
            return Ok(new { message = "Strategy submitted for approval", submissionId });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error submitting strategy" });
        }
    }

    [HttpPost("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitCategory([FromBody] CategorySubmissionRequest request)
    {
        try
        {
            InputValidator.ValidateCategorySubmissionRequest(request);
            int submissionId = await _submissionService.SubmitCategory(request);
            return Ok(new { message = "Category submitted for approval", submissionId });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error submitting category" });
        }
    }

    // Admin-only endpoints.
    [RequireAdminApiKey]
    [HttpGet("admin/strats")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StratSubmission>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPendingStrats()
    {
        return Ok(await _submissionService.GetPendingStratSubmissions());
    }

    [RequireAdminApiKey]
    [HttpGet("admin/strats/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StratSubmission))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPendingStrat(int id)
    {
        StratSubmission? submission = await _submissionService.GetPendingStratSubmission(id);
        if (submission == null)
            return NotFound(new { message = $"Strategy submission by ID {id} was not found" });

        return Ok(submission);
    }

    [RequireAdminApiKey]
    [HttpPost("admin/strats/{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ApproveStrat(int id)
    {
        try
        {
            int? stratId = await _submissionService.ApproveStratSubmission(id);
            if (stratId == null)
                return NotFound(new { message = $"Strategy submission by ID {id} was not found" });

            return Ok(new { message = "Strategy submission approved", stratId });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error approving strategy submission" });
        }
    }

    [RequireAdminApiKey]
    [HttpDelete("admin/strats/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RejectStrat(int id)
    {
        if (!await _submissionService.RejectStratSubmission(id))
            return NotFound(new { message = $"Strategy submission by ID {id} was not found" });

        return Ok(new { message = "Strategy submission rejected" });
    }

    // Admin-only endpoints.
    [RequireAdminApiKey]
    [HttpGet("admin/categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategorySubmission>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPendingCategories()
    {
        return Ok(await _submissionService.GetPendingCategorySubmissions());
    }

    [RequireAdminApiKey]
    [HttpGet("admin/categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategorySubmission))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPendingCategory(int id)
    {
        CategorySubmission? submission = await _submissionService.GetPendingCategorySubmission(id);
        if (submission == null)
            return NotFound(new { message = $"Category submission by ID {id} was not found" });

        return Ok(submission);
    }

    [RequireAdminApiKey]
    [HttpPost("admin/categories/{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ApproveCategory(int id)
    {
        try
        {
            int? categoryId = await _submissionService.ApproveCategorySubmission(id);
            if (categoryId == null)
                return NotFound(new { message = $"Category submission by ID {id} was not found" });

            return Ok(new { message = "Category submission approved", categoryId });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error approving category submission" });
        }
    }

    [RequireAdminApiKey]
    [HttpDelete("admin/categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RejectCategory(int id)
    {
        if (!await _submissionService.RejectCategorySubmission(id))
            return NotFound(new { message = $"Category submission by ID {id} was not found" });

        return Ok(new { message = "Category submission rejected" });
    }
}
