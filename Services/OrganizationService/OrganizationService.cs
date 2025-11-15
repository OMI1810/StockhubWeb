using StockhubWeb.Models;

namespace StockhubWeb.Services.OrganizationService
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationApiService _organizationApiService;
        private Organization? _currentOrganization;

        public OrganizationService(IOrganizationApiService organizationApiService)
        {
            _organizationApiService = organizationApiService;
        }

        public async Task<List<Organization>> GetUserOrganizationsAsync()
        {
            var result = await _organizationApiService.GetUserOrganizationsAsync();
            return result.Success ? result.Data ?? new List<Organization>() : new List<Organization>();
        }

        public async Task<Organization?> CreateOrganizationAsync(string name)
        {
            var model = new CreateOrganizationModel { Name = name };
            var result = await _organizationApiService.CreateOrganizationAsync(model);

            if (result.Success && result.Data != null)
            {
                // Автоматически выбираем созданную организацию
                await SelectOrganizationAsync(result.Data.OrganizationId);
                return result.Data;
            }

            return null;
        }

        public async Task<bool> SelectOrganizationAsync(string organizationId)
        {
            var result = await _organizationApiService.SelectOrganizationAsync(organizationId);

            if (result.Success && result.Data?.Organization != null)
            {
                _currentOrganization = result.Data.Organization;
                return true;
            }

            return false;
        }

        public async Task<Organization?> GetCurrentOrganizationAsync()
        {
            // Сначала проверяем локальное состояние
            if (_currentOrganization != null)
                return _currentOrganization;

            // Если локально нет, запрашиваем у сервера
            var result = await _organizationApiService.GetCurrentOrganizationAsync();

            if (result.Success && result.Data != null && !string.IsNullOrEmpty(result.Data.OrganizationId))
            {
                _currentOrganization = new Organization
                {
                    OrganizationId = result.Data.OrganizationId,
                    Name = result.Data.Name ?? string.Empty,
                    Role = result.Data.Role ?? string.Empty
                };
            }

            return _currentOrganization;
        }

        public async Task<bool> DeleteOrganizationAsync(string organizationId)
        {
            var result = await _organizationApiService.DeleteOrganizationAsync(organizationId);

            if (result.Success)
            {
                // Если удаляем текущую организацию, сбрасываем состояние
                if (_currentOrganization?.OrganizationId == organizationId)
                {
                    _currentOrganization = null;
                }
                return true;
            }

            return false;
        }

        public Task SetCurrentOrganizationAsync(Organization organization)
        {
            _currentOrganization = organization;
            return Task.CompletedTask;
        }
    }
}