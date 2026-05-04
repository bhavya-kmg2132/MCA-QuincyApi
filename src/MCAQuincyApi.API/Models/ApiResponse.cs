namespace MCAQuincyApi.API.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public long DurationMs { get; set; }
    public int Count { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(string message, long durationMs, T? data, int count)
        => new()
        {
            Success = true,
            Message = message,
            ErrorCode = null,
            DurationMs = durationMs,
            Count = count,
            Data = data
        };

    public static ApiResponse<T> ErrorResponse(string message, string errorCode, long durationMs, T? data = default)
        => new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            DurationMs = durationMs,
            Count = 0,
            Data = data
        };
}
