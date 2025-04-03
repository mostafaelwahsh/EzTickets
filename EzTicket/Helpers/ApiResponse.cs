public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data, string message = "", bool success = true)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new ApiResponse<T>(data, message, true);

    public static ApiResponse<T> Fail(string message = "Something went wrong") =>
        new ApiResponse<T> { Success = false, Message = message, Data = default };
}
