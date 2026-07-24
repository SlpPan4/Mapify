namespace MapifyBackend.Utility.Api;

/// <summary>
/// Generic API response envelope.
/// </summary>
/// <typeparam name="T">Type of the response payload.</typeparam>
public class ApiResponse<T>
{
    public int Status { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Non-generic API response envelope for messages and untyped payloads.
/// </summary>
public class ApiResponse
{
    public int Status { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }

    public static ApiResponse<T> Success<T>(T data, string? message = null, int status = StatusCodes.Status200OK)
    {
        return new ApiResponse<T>
        {
            Status = status,
            Data = data,
            Message = message,
            Error = null
        };
    }

    public static ApiResponse SuccessMessage(string? message = null, int status = StatusCodes.Status200OK)
    {
        return new ApiResponse
        {
            Status = status,
            Data = null,
            Message = message,
            Error = null
        };
    }

    public static ApiResponse Failure(int status, string error, string? message = null)
    {
        return new ApiResponse
        {
            Status = status,
            Data = null,
            Message = message,
            Error = error
        };
    }

    public static ApiResponse<T> FailureFor<T>(int status, string error, string? message = null)
    {
        return new ApiResponse<T>
        {
            Status = status,
            Data = default,
            Message = message,
            Error = error
        };
    }

    public static ApiResponse NotFound(string error, string? message = null)
        => Failure(StatusCodes.Status404NotFound, error, message);

    public static ApiResponse BadRequest(string error, string? message = null)
        => Failure(StatusCodes.Status400BadRequest, error, message);

    public static ApiResponse<T> Created<T>(T data, string? message = null)
        => Success(data, message, StatusCodes.Status201Created);
}
