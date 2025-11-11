using StockhubWeb.Models;

namespace StockhubWeb.Services.OrganizationService
{
    public interface IOrganizationService
    {
        Task<Organization?> GetUserOrganizationAsync(string userId);
        Task<bool> CreateOrganizationAsync(string name, string password, string ownerId);
        Task<bool> AddEmployeeToOrganizationAsync(string organizationId, string employeeId);
        Task<bool> JoinOrganizationAsync(string organizationId, string password, string userId);
        Task<List<Organization>> GetUserOrganizationsAsync(string userId);
        Task<Organization?> GetOrganizationByIdAsync(string organizationId);
    }
}