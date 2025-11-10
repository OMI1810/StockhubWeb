using stockhub-web.Models;

namespace stockhub-web.Services
{
    public class MockOrganizationService : IOrganizationService
    {
        private static readonly List<Organization> _organizations = new();
        private static readonly List<Warehouse> _warehouses = new();
        private static int _nextOrgId = 1;
        private static int _nextWarehouseId = 1;

        public Task<Organization?> CreateOrganizationAsync(string name, string password, int organizerId)
        {
            var organization = new Organization
            {
                Id = _nextOrgId++,
                Name = name,
                OrganizationPassword = password,
                OrganizerId = organizerId
            };

            _organizations.Add(organization);
            return Task.FromResult<Organization?>(organization);
        }

        public Task<bool> InviteEmployeeAsync(string email, int organizationId)
        {
            // В реальном приложении здесь была бы отправка приглашения
            Console.WriteLine($"Invitation sent to {email} for organization {organizationId}");
            return Task.FromResult(true);
        }

        public Task<Warehouse?> CreateWarehouseAsync(WarehouseViewModel model, int organizationId)
        {
            var warehouse = new Warehouse
            {
                Id = _nextWarehouseId++,
                Name = model.Name,
                IsActive = model.IsActive,
                OrganizationId = organizationId
            };

            _warehouses.Add(warehouse);
            return Task.FromResult<Warehouse?>(warehouse);
        }

        public Task<List<Warehouse>> GetUserWarehousesAsync(int userId)
        {
            var userOrg = _organizations.FirstOrDefault(o => o.OrganizerId == userId);
            if (userOrg != null)
            {
                var warehouses = _warehouses.Where(w => w.OrganizationId == userOrg.Id).ToList();
                return Task.FromResult(warehouses);
            }

            return Task.FromResult(new List<Warehouse>());
        }

        public static Organization? GetOrganizationByUser(int userId)
        {
            return _organizations.FirstOrDefault(o => o.OrganizerId == userId);
        }
    }
}