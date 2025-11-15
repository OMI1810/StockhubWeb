namespace StockhubWeb.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }
    }

    public class AuthResponse
    {
        public User? User { get; set; }
        public string? Message { get; set; }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
        public List<string>? Errors { get; set; }
    }
}