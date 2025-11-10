using stockhub-web.Models;

namespace stockhub-web.Models
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> RegisterAsync(RegisterViewModel model);
        Task<bool> SendPasswordResetCodeAsync(string email);
        Task<bool> ValidateResetCodeAsync(string email, string code);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
    }

    public interface IOrganizationService
    {
        Task<Organization?> CreateOrganizationAsync(string name, string password, int organizerId);
        Task<bool> InviteEmployeeAsync(string email, int organizationId);
        Task<Warehouse?> CreateWarehouseAsync(WarehouseViewModel model, int organizationId);
        Task<List<Warehouse>> GetUserWarehousesAsync(int userId);
    }
}