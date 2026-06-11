namespace AircraftMRO.Common.Results;

public class ServiceResult<T>
{
    public bool Success { get; set; }

    public T? Data { get; set; }

    // General/business errors
    public string? ErrorMessage { get; set; }

    // Validation errors
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    public static ServiceResult<T> SuccessResult(T data)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static ServiceResult<T> ValidationFailure(
        Dictionary<string, string[]> validationErrors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            ValidationErrors = validationErrors
        };
    }
}