using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.DTOs;
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
    public IActionResult SubmitStrat([FromBody] StratSubmissionRequest request)
    {
        try
        {
            InputValidator.ValidateStratSubmissionRequest(request);
            int submissionId = _submissionService.SubmitStrat(request);
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
    public IActionResult SubmitCategory([FromBody] CategorySubmissionRequest request)
    {
        try
        {
            InputValidator.ValidateCategorySubmissionRequest(request);
            int submissionId = _submissionService.SubmitCategory(request);
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

    // Admin-only endpoints. Restrict this route at the hosting/auth layer before exposing it publicly.
    [HttpGet("admin/strats")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StratSubmission>))]
    public IActionResult GetPendingStrats()
    {
        return Ok(_submissionService.GetPendingStratSubmissions());
    }

    [HttpGet("admin/strats/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StratSubmission))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPendingStrat(int id)
    {
        StratSubmission? submission = _submissionService.GetPendingStratSubmission(id);
        if (submission == null)
            return NotFound(new { message = $"Strategy submission by ID {id} was not found" });

        return Ok(submission);
    }

    [HttpPost("admin/strats/{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ApproveStrat(int id)
    {
        try
        {
            int? stratId = _submissionService.ApproveStratSubmission(id);
            if (stratId == null)
                return NotFound(new { message = $"Strategy submission by ID {id} was not found" });

            return Ok(new { message = "Strategy submission approved", stratId });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error approving strategy submission" });
        }
    }

    [HttpDelete("admin/strats/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RejectStrat(int id)
    {
        if (!_submissionService.RejectStratSubmission(id))
            return NotFound(new { message = $"Strategy submission by ID {id} was not found" });

        return Ok(new { message = "Strategy submission rejected" });
    }

    // Admin-only endpoints. Restrict this route at the hosting/auth layer before exposing it publicly.
    [HttpGet("admin/categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategorySubmission>))]
    public IActionResult GetPendingCategories()
    {
        return Ok(_submissionService.GetPendingCategorySubmissions());
    }

    [HttpGet("admin/categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategorySubmission))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPendingCategory(int id)
    {
        CategorySubmission? submission = _submissionService.GetPendingCategorySubmission(id);
        if (submission == null)
            return NotFound(new { message = $"Category submission by ID {id} was not found" });

        return Ok(submission);
    }

    [HttpPost("admin/categories/{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ApproveCategory(int id)
    {
        try
        {
            int? categoryId = _submissionService.ApproveCategorySubmission(id);
            if (categoryId == null)
                return NotFound(new { message = $"Category submission by ID {id} was not found" });

            return Ok(new { message = "Category submission approved", categoryId });
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Error approving category submission" });
        }
    }

    [HttpDelete("admin/categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RejectCategory(int id)
    {
        if (!_submissionService.RejectCategorySubmission(id))
            return NotFound(new { message = $"Category submission by ID {id} was not found" });

        return Ok(new { message = "Category submission rejected" });
    }
}
