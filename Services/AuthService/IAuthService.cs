using StockhubWeb.Models;

namespace StockhubWeb.Services.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginModel loginModel);
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterModel registerModel);
        Task<ApiResponse<AuthResponse>> ConfirmEmailAsync(EmailConfirmationModel confirmationModel);
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
        Task SetCurrentUserAsync(User user);
    }
}