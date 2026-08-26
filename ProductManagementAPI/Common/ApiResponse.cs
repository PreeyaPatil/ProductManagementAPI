namespace ProductManagementAPI.Common
{ 
// Defines a common response structure for all API operations.
// T represents the type of data returned by the API,
// such as ProductResponseDTO or PagedResult<ProductResponseDTO>.
public class ApiResponse<T>
{
    // Prevents objects from being created directly from outside the class.
    // Responses must be created using CreateSuccess() or CreateFailure().
    private ApiResponse()
    {
    }

    // Indicates whether the API operation was successful.
    public bool Success { get; init; }

    // Contains the HTTP status code, such as 200, 400, 404, or 500.
    public int StatusCode { get; init; }

    // Contains a user-friendly message describing the result.
    public string Message { get; init; } = string.Empty;

    // Contains the data returned by a successful operation.
    // It can be null when no data needs to be returned.
    public T? Data { get; init; }

    // Contains validation or other error details.
    // The key normally represents a field name, while the string array
    // contains one or more error messages related to that field.
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    // Contains the unique identifier of the current HTTP request.
    // It helps connect an API response with the related application logs.
    public string? TraceId { get; init; }

    // Records the UTC date and time when the response was created.
    public DateTime Timestamp { get; init; }

    // Creates and returns a successful API response.
    public static ApiResponse<T> CreateSuccess(
        int statusCode,
        string message,
        T? data,
        string? traceId)
    {
        return new ApiResponse<T>
        {
            // Marks the operation as successful.
            Success = true,

            // Stores the successful HTTP status code.
            StatusCode = statusCode,

            // Stores the success message.
            Message = message,

            // Stores the data returned by the operation.
            Data = data,

            // A successful response does not contain errors.
            Errors = null,

            // Stores the request's trace identifier.
            TraceId = traceId,

            // Stores the response creation time in UTC.
            Timestamp = DateTime.UtcNow
        };
    }

    // Creates and returns a failed API response.
    public static ApiResponse<T> CreateFailure(
        int statusCode,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null,
        string? traceId = null)
    {
        return new ApiResponse<T>
        {
            // Marks the operation as unsuccessful.
            Success = false,

            // Stores the error HTTP status code.
            StatusCode = statusCode,

            // Stores the error message.
            Message = message,

            // A failed response does not return normal result data.
            // default will be null for reference and nullable types.
            Data = default,

            // Stores validation or other error details when available.
            Errors = errors,

            // Stores the request's trace identifier.
            TraceId = traceId,

            // Stores the response creation time in UTC.
            Timestamp = DateTime.UtcNow
        };
    }
}
}
