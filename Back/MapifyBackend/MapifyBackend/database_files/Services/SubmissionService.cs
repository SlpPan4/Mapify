using MapifyBackend.Utility.DTOs;
using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

public class SubmissionService
{
    private readonly DatabaseService _db;

    public SubmissionService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<int> SubmitStrat(StratSubmissionRequest request)
    {
        int? mapId = await _db.GetMapIdByNameAsync(request.MapName);
        if (mapId == null)
            throw new ArgumentException($"Map '{request.MapName}' does not exist");

        List<int> categoryIds = request.CategoryIds.Distinct().ToList();
        foreach (int categoryId in categoryIds)
        {
            if (await _db.GetCategoryByIdAsync(categoryId) == null)
                throw new ArgumentException($"Category by id {categoryId} does not exist");
        }

        List<int> operatorIds = request.OperatorIds.Distinct().ToList();
        foreach (int operatorId in operatorIds)
        {
            if (await _db.GetOperatorByIdAsync(operatorId) == null)
                throw new ArgumentException($"Operator by id {operatorId} does not exist");
        }

        StratSubmission submission = new StratSubmission(
            request.Name,
            request.VideoUrl,
            mapId.Value,
            request.Description,
            categoryIds,
            operatorIds);

        return await _db.AddPendingStratSubmissionAsync(submission);
    }

    public async Task<int> SubmitCategory(CategorySubmissionRequest request)
    {
        if (await _db.GetCategoryIdByNameAsync(request.Name) != null)
            throw new InvalidOperationException($"Category '{request.Name}' already exists");

        Side side = Enum.Parse<Side>(request.Side);
        CategorySubmission submission = new CategorySubmission(request.Name, side);
        return await _db.AddPendingCategorySubmissionAsync(submission);
    }

    public async Task<List<StratSubmission>> GetPendingStratSubmissions()
    {
        return await _db.GetPendingStratSubmissionsAsync();
    }

    public async Task<StratSubmission?> GetPendingStratSubmission(int id)
    {
        return await _db.GetPendingStratSubmissionByIdAsync(id);
    }

    public async Task<int?> ApproveStratSubmission(int id)
    {
        return await _db.ApprovePendingStratSubmissionAsync(id);
    }

    public async Task<bool> RejectStratSubmission(int id)
    {
        return await _db.DeletePendingStratSubmissionAsync(id);
    }

    public async Task<List<CategorySubmission>> GetPendingCategorySubmissions()
    {
        return await _db.GetPendingCategorySubmissionsAsync();
    }

    public async Task<CategorySubmission?> GetPendingCategorySubmission(int id)
    {
        return await _db.GetPendingCategorySubmissionByIdAsync(id);
    }

    public async Task<int?> ApproveCategorySubmission(int id)
    {
        return await _db.ApprovePendingCategorySubmissionAsync(id);
    }

    public async Task<bool> RejectCategorySubmission(int id)
    {
        return await _db.DeletePendingCategorySubmissionAsync(id);
    }
}
