using MapifyBackend.Utility;
using MapifyBackend.Utility.DTOs;
using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

/// <summary>
/// Service layer for managing public strategy/category submissions and admin review.
/// </summary>
public class SubmissionService
{
    private readonly DatabaseService _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmissionService"/> class.
    /// </summary>
    /// <param name="db">The database service used for data persistence.</param>
    public SubmissionService(DatabaseService db)
    {
        _db = db;
    }

    /// <summary>
    /// Submits a new strategy for admin approval.
    /// </summary>
    /// <param name="request">The strategy submission request.</param>
    /// <returns>The ID of the created pending submission.</returns>
    /// <exception cref="ArgumentException">Thrown when the map, category, or operator does not exist.</exception>
    /// <exception cref="ValidationException">Thrown when selected categories or operators have mismatched sides.</exception>
    public async Task<int> SubmitStrat(StratSubmissionRequest request)
    {
        Map? map = await _db.GetMapById(request.MapId);
        if (map == null)
            throw new ArgumentException($"Map by id {request.MapId} does not exist");

        List<int> categoryIds = request.CategoryIds.Distinct().ToList();
        foreach (int categoryId in categoryIds)
        {
            if (await _db.GetCategoryById(categoryId) == null)
                throw new ArgumentException($"Category by id {categoryId} does not exist");
        }

        List<int> operatorIds = request.OperatorIds.Distinct().ToList();
        foreach (int operatorId in operatorIds)
        {
            if (await _db.GetOperatorById(operatorId) == null)
                throw new ArgumentException($"Operator by id {operatorId} does not exist");
        }

        // Validate that selected categories and operators belong to consistent sides.
        List<Category> categories = await _db.GetCategoriesByIds(categoryIds);
        List<Operator> operators = await _db.GetOperatorsByIds(operatorIds);

        Side? expectedSide = null;

        if (categories.Count > 0)
        {
            expectedSide = categories[0].Side;
            if (categories.Any(c => c.Side != expectedSide))
                throw new ValidationException("All selected categories must belong to the same side");
        }

        if (operators.Count > 0)
        {
            Side operatorSide = operators[0].Side;
            if (operators.Any(o => o.Side != operatorSide))
                throw new ValidationException("All selected operators must belong to the same side");

            if (expectedSide.HasValue && operatorSide != expectedSide.Value)
                throw new ValidationException(
                    $"Selected operators belong to {operatorSide}, but selected categories belong to {expectedSide.Value}");
        }

        StratSubmission submission = new StratSubmission(
            request.Name,
            request.VideoUrl,
            request.MapId,
            request.Description,
            categoryIds,
            operatorIds);

        return await _db.AddPendingStratSubmission(submission);
    }

    /// <summary>
    /// Submits a new category for admin approval.
    /// </summary>
    /// <param name="request">The category submission request.</param>
    /// <returns>The ID of the created pending submission.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a category with the same name already exists.</exception>
    public async Task<int> SubmitCategory(CategorySubmissionRequest request)
    {
        if (await _db.GetCategoryIdByName(request.Name) != null)
            throw new InvalidOperationException($"Category '{request.Name}' already exists");

        Side side = Enum.Parse<Side>(request.Side);
        CategorySubmission submission = new CategorySubmission(request.Name, side);
        return await _db.AddPendingCategorySubmission(submission);
    }

    /// <summary>
    /// Retrieves all pending strategy submissions.
    /// </summary>
    /// <returns>A list of pending strategy submissions.</returns>
    public async Task<List<StratSubmission>> GetPendingStratSubmissions()
    {
        return await _db.GetPendingStratSubmissions();
    }

    /// <summary>
    /// Retrieves a pending strategy submission by ID.
    /// </summary>
    /// <param name="id">The pending submission ID.</param>
    /// <returns>The pending submission if found; otherwise, null.</returns>
    public async Task<StratSubmission?> GetPendingStratSubmission(int id)
    {
        return await _db.GetPendingStratSubmissionById(id);
    }

    /// <summary>
    /// Approves a pending strategy submission and moves it to the public strategies table.
    /// </summary>
    /// <param name="id">The pending submission ID.</param>
    /// <returns>The ID of the approved strategy if found; otherwise, null.</returns>
    public async Task<int?> ApproveStratSubmission(int id)
    {
        return await _db.ApprovePendingStratSubmission(id);
    }

    /// <summary>
    /// Rejects a pending strategy submission.
    /// </summary>
    /// <param name="id">The pending submission ID.</param>
    /// <returns>True if the submission was found and rejected; otherwise, false.</returns>
    public async Task<bool> RejectStratSubmission(int id)
    {
        return await _db.DeletePendingStratSubmission(id);
    }

    /// <summary>
    /// Retrieves all pending category submissions.
    /// </summary>
    /// <returns>A list of pending category submissions.</returns>
    public async Task<List<CategorySubmission>> GetPendingCategorySubmissions()
    {
        return await _db.GetPendingCategorySubmissions();
    }

    /// <summary>
    /// Retrieves a pending category submission by ID.
    /// </summary>
    /// <param name="id">The pending submission ID.</param>
    /// <returns>The pending submission if found; otherwise, null.</returns>
    public async Task<CategorySubmission?> GetPendingCategorySubmission(int id)
    {
        return await _db.GetPendingCategorySubmissionById(id);
    }

    /// <summary>
    /// Approves a pending category submission and moves it to the public categories table.
    /// </summary>
    /// <param name="id">The pending submission ID.</param>
    /// <returns>The ID of the approved category if found; otherwise, null.</returns>
    public async Task<int?> ApproveCategorySubmission(int id)
    {
        return await _db.ApprovePendingCategorySubmission(id);
    }

    /// <summary>
    /// Rejects a pending category submission.
    /// </summary>
    /// <param name="id">The pending submission ID.</param>
    /// <returns>True if the submission was found and rejected; otherwise, false.</returns>
    public async Task<bool> RejectCategorySubmission(int id)
    {
        return await _db.DeletePendingCategorySubmission(id);
    }
}
