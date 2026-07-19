using MapifyBackend.Utility.DTOs;

namespace MapifyBackend.Utility;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public static class InputValidator
{
    private static void ValidateString(string value, string fieldName, int maxLength = 500)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{fieldName} is required");

        if (value.Length > maxLength)
            throw new ValidationException($"{fieldName} must be less than {maxLength} characters");
    }

    private static void ValidateOptionalString(string? value, string fieldName, int maxLength = 500)
    {
        if (value is null)
            return;

        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{fieldName} cannot be empty when provided");

        if (value.Length > maxLength)
            throw new ValidationException($"{fieldName} must be less than {maxLength} characters");
    }

    public static void ValidateStratRequest(StratRequest request)
    {
        ValidateString(request.Name, "Name", 100);
        ValidateString(request.VideoUrl, "VideoUrl", 500);
        ValidateString(request.MapName, "MapName", 100);
    }

    public static void ValidateStratUpdateRequest(StratUpdateRequest request)
    {
        ValidateString(request.Name, "Name", 100);
        ValidateString(request.VideoUrl, "VideoUrl", 500);
        ValidateString(request.MapName, "MapName", 100);

        if (request.Description is { Length: > 1000 })
            throw new ValidationException("Description must be less than 1000 characters");
    }

    public static void ValidateStratPatchRequest(StratPatchRequest request)
    {
        ValidateOptionalString(request.Name, "Name", 100);
        ValidateOptionalString(request.VideoUrl, "VideoUrl", 500);
        ValidateOptionalString(request.MapName, "MapName", 100);

        if (request.Description is { Length: > 1000 })
            throw new ValidationException("Description must be less than 1000 characters");
    }

    public static void ValidateCategoryRequest(CategoryRequest request)
    {
        ValidateString(request.Name, "Name", 100);
        ValidateString(request.Side, "Side", 7);
        if (request.Side != "Attack" && request.Side != "Defense")
        {
            throw new ValidationException("Side must be only 'Attack' or 'Defense'");
        }
    }

    public static void ValidateStratSubmissionRequest(StratSubmissionRequest request)
    {
        ValidateString(request.Name, "Name", 100);
        ValidateString(request.VideoUrl, "VideoUrl", 500);
        ValidateString(request.MapName, "MapName", 100);

        if (request.Description is { Length: > 1000 })
            throw new ValidationException("Description must be less than 1000 characters");

        if (request.CategoryIds.Any(id => id <= 0))
            throw new ValidationException("CategoryIds must contain only positive IDs");

        if (request.OperatorIds.Any(id => id <= 0))
            throw new ValidationException("OperatorIds must contain only positive IDs");
    }

    public static void ValidateCategorySubmissionRequest(CategorySubmissionRequest request)
    {
        ValidateCategoryRequest(new CategoryRequest
        {
            Name = request.Name,
            Side = request.Side
        });
    }
}
