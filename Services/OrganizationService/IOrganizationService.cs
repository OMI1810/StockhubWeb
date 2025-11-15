using StockhubWeb.Models;

namespace StockhubWeb.Services.OrganizationService
{
    public interface IOrganizationService
    {
        Task<List<Organization>> GetUserOrganizationsAsync();
        Task<Organization?> CreateOrganizationAsync(string name);
        Task<bool> SelectOrganizationAsync(string organizationId);
        Task<Organization?> GetCurrentOrganizationAsync();
        Task<bool> DeleteOrganizationAsync(string organizationId);
        Task SetCurrentOrganizationAsync(Organization organization);
    }
}