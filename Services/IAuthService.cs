// Services/IAuthService.cs
using WarehouseApp.Models;

namespace stockhub_web.Models
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

// Services/MockAuthService.cs
using WarehouseApp.Models;

namespace WarehouseApp.Services
{
    public class MockAuthService : IAuthService
    {
        private static readonly Dictionary<string, User> _users = new();
        private static readonly Dictionary<string, string> _resetCodes = new();

        public Task<User?> AuthenticateAsync(string email, string password)
        {
            if (_users.TryGetValue(email, out var user) && user.Password == password)
                return Task.FromResult<User?>(user);

            return Task.FromResult<User?>(null);
        }

        public Task<User?> RegisterAsync(RegisterViewModel model)
        {
            if (_users.ContainsKey(model.Email))
                return Task.FromResult<User?>(null);

            var user = new User
            {
                Id = _users.Count + 1,
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                MiddleName = model.NoMiddleName ? null : model.MiddleName,
                Role = UserRole.Employee
            };

            _users[model.Email] = user;
            return Task.FromResult<User?>(user);
        }

        public Task<bool> SendPasswordResetCodeAsync(string email)
        {
            if (!_users.ContainsKey(email))
                return Task.FromResult(false);

            var code = new Random().Next(100000, 999999).ToString();
            _resetCodes[email] = code;

            // В реальном приложении здесь была бы отправка email
            Console.WriteLine($"Reset code for {email}: {code}");

            return Task.FromResult(true);
        }

        public Task<bool> ValidateResetCodeAsync(string email, string code)
        {
            return Task.FromResult(_resetCodes.TryGetValue(email, out var storedCode) && storedCode == code);
        }

        public Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            if (!_users.ContainsKey(email))
                return Task.FromResult(false);

            _users[email].Password = newPassword;
            _resetCodes.Remove(email);
            return Task.FromResult(true);
        }
    }
}

// Services/MockOrganizationService.cs
using WarehouseApp.Models;

namespace WarehouseApp.Services
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