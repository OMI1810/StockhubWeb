using StockhubWeb.Models;

namespace StockhubWeb.Services.AuthService
{
    public interface IAuthService
    {
        Task<bool> LoginUserAsync(LoginModel request);
        Task<bool> RegisterUserAsync(RegisterModel request);
        Task<bool> InviteEmployeeAsync(InviteEmployeeModel request);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
        Task<List<User>> GetOrganizationEmployeesAsync(string organizationId);
        Task<bool> RemoveEmployeeAsync(string employeeId);
    }
}