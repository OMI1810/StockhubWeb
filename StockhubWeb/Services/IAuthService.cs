using StockhubWeb.Models;

namespace StockhubWeb.Services
{
    public interface IAuthService
    {
        Task<bool> LoginUserAsync(LoginModel request);
        Task<bool> RegisterUserAsync(RegisterModel request);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
    }
}