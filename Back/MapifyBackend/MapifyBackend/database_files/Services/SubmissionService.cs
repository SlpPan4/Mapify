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

    public int SubmitStrat(StratSubmissionRequest request)
    {
        int? mapId = _db.GetMapIdByName(request.MapName);
        if (mapId == null)
            throw new ArgumentException($"Map '{request.MapName}' does not exist");

        List<int> categoryIds = request.CategoryIds.Distinct().ToList();
        foreach (int categoryId in categoryIds)
        {
            if (_db.GetCategoryById(categoryId) == null)
                throw new ArgumentException($"Category by id {categoryId} does not exist");
        }

        List<int> operatorIds = request.OperatorIds.Distinct().ToList();
        foreach (int operatorId in operatorIds)
        {
            if (_db.GetOperatorById(operatorId) == null)
                throw new ArgumentException($"Operator by id {operatorId} does not exist");
        }

        StratSubmission submission = new StratSubmission(
            request.Name,
            request.VideoUrl,
            mapId.Value,
            request.Description,
            categoryIds,
            operatorIds);

        return _db.AddPendingStratSubmission(submission);
    }

    public int SubmitCategory(CategorySubmissionRequest request)
    {
        if (_db.GetCategoryIdByName(request.Name) != null)
            throw new InvalidOperationException($"Category '{request.Name}' already exists");

        Side side = Enum.Parse<Side>(request.Side);
        CategorySubmission submission = new CategorySubmission(request.Name, side);
        return _db.AddPendingCategorySubmission(submission);
    }

    public List<StratSubmission> GetPendingStratSubmissions()
    {
        return _db.GetPendingStratSubmissions();
    }

    public StratSubmission? GetPendingStratSubmission(int id)
    {
        return _db.GetPendingStratSubmissionById(id);
    }

    public int? ApproveStratSubmission(int id)
    {
        return _db.ApprovePendingStratSubmission(id);
    }

    public bool RejectStratSubmission(int id)
    {
        return _db.DeletePendingStratSubmission(id);
    }

    public List<CategorySubmission> GetPendingCategorySubmissions()
    {
        return _db.GetPendingCategorySubmissions();
    }

    public CategorySubmission? GetPendingCategorySubmission(int id)
    {
        return _db.GetPendingCategorySubmissionById(id);
    }

    public int? ApproveCategorySubmission(int id)
    {
        return _db.ApprovePendingCategorySubmission(id);
    }

    public bool RejectCategorySubmission(int id)
    {
        return _db.DeletePendingCategorySubmission(id);
    }
}
