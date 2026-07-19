using MapifyBackend.database_files;
using MapifyBackend.Utility;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using MapifyBackend.Utility.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MapifyBackend.Controllers;

/// <summary>
/// API for managing strategy categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly DatabaseService _db;
    private readonly CategoryService _categoryService;

    public CategoriesController(DatabaseService db, CategoryService categoryService)
    {
        _db = db;
        _categoryService = categoryService;
    }

    /// <summary>
    /// Returns all categories in the system.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Category>>))]
    public async Task<IActionResult> GetAll()
    {
        var allCategories = await _categoryService.GetAllCategories();
        return Ok(ApiResponse.Success(allCategories));
    }

    /// <summary>
    /// Returns a category by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Category>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetById(int id)
    {
        Category? category = await _categoryService.GetCategoryById(id);
        if (category == null)
        {
            return NotFound(ApiResponse.NotFound($"Category by ID {id} was not found"));
        }

        return Ok(ApiResponse.Success(category));
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request)
    {
        try
        {
            InputValidator.ValidateCategoryRequest(request);
            Side side = Enum.Parse<Side>(request.Side);
            await _categoryService.AddCategory(request.Name, side);
            return Ok(ApiResponse.SuccessMessage("Category added"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.BadRequest(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse.BadRequest("Error adding category"));
        }
    }

    /// <summary>
    /// Deletes a category by ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _categoryService.DeleteCategory(id))
            return NotFound(ApiResponse.NotFound($"Category by id {id} was not found"));

        return Ok(ApiResponse.SuccessMessage("Category deleted"));
    }

    /// <summary>
    /// Returns the name of a category by ID.
    /// </summary>
    [HttpGet("category_name/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetName(int id)
    {
        string? name = await _categoryService.GetCategoryNameById(id);
        if (name == null)
            return NotFound(ApiResponse.NotFound($"Category by id {id} was not found"));

        return Ok(ApiResponse.Success(new { name }));
    }
}
