// OrganizationService.cs
using StockhubWeb.Models;

namespace StockhubWeb.Services.OrganizationService
{
    public class OrganizationService : IOrganizationService
    {
        private readonly List<Organization> _organizations = new();
        private readonly List<OrganizationMember> _members = new();

        public OrganizationService()
        {
            // Создаем тестовую организацию для демонстрации
            CreateTestOrganization();
        }

        private void CreateTestOrganization()
        {
            var testOrganization = new Organization
            {
                Id = "test-org-id",
                Name = "Тестовая организация",
                OwnerId = "test-organizer-id",
                CreatedAt = DateTime.Now.AddDays(-30)
            };

            _organizations.Add(testOrganization);

            // Добавляем владельца
            _members.Add(new OrganizationMember
            {
                UserId = "test-organizer-id",
                OrganizationId = "test-org-id",
                Role = "Owner"
            });

            // Добавляем тестового сотрудника
            _members.Add(new OrganizationMember
            {
                UserId = "test-employee-id",
                OrganizationId = "test-org-id",
                Role = "Employee"
            });

            testOrganization.EmployeeIds.Add("test-employee-id");
        }

        public async Task<Organization?> GetUserOrganizationAsync(string userId)
        {
            await Task.Delay(100);
            var member = _members.FirstOrDefault(m => m.UserId == userId);
            return member != null ? _organizations.FirstOrDefault(o => o.Id == member.OrganizationId) : null;
        }

        public async Task<bool> CreateOrganizationAsync(string name, string ownerId)
        {
            await Task.Delay(500);

            // Проверяем, не является ли пользователь уже владельцем организации с таким названием
            var existingOrganization = _organizations.FirstOrDefault(o =>
                o.Name == name && _members.Any(m =>
                    m.OrganizationId == o.Id &&
                    m.UserId == ownerId &&
                    m.Role == "Owner"));

            if (existingOrganization != null)
            {
                // Организация с таким названием уже существует у этого пользователя
                return false;
            }

            var organization = new Organization
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                OwnerId = ownerId,
                CreatedAt = DateTime.Now
            };

            _organizations.Add(organization);

            // Добавляем владельца в организацию
            _members.Add(new OrganizationMember
            {
                UserId = ownerId,
                OrganizationId = organization.Id,
                Role = "Owner"
            });

            return true;
        }

        public async Task<bool> AddEmployeeToOrganizationAsync(string organizationId, string employeeId)
        {
            await Task.Delay(500);

            var organization = _organizations.FirstOrDefault(o => o.Id == organizationId);
            if (organization != null)
            {
                // Проверяем, не является ли пользователь уже членом организации
                var existingMember = _members.FirstOrDefault(m =>
                    m.UserId == employeeId && m.OrganizationId == organizationId);

                if (existingMember == null)
                {
                    _members.Add(new OrganizationMember
                    {
                        UserId = employeeId,
                        OrganizationId = organizationId,
                        Role = "Employee"
                    });

                    organization.EmployeeIds.Add(employeeId);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> JoinOrganizationAsync(string organizationId, string userId)
        {
            await Task.Delay(500);

            var organization = _organizations.FirstOrDefault(o =>
                o.Id == organizationId);

            if (organization != null)
            {
                // Проверяем, не является ли пользователь уже членом организации
                var existingMember = _members.FirstOrDefault(m =>
                    m.UserId == userId && m.OrganizationId == organizationId);

                if (existingMember == null)
                {
                    _members.Add(new OrganizationMember
                    {
                        UserId = userId,
                        OrganizationId = organizationId,
                        Role = "Employee"
                    });

                    organization.EmployeeIds.Add(userId);
                    return true;
                }
            }
            return false;
        }

        public async Task<List<Organization>> GetUserOrganizationsAsync(string userId)
        {
            await Task.Delay(100);
            var userOrgIds = _members
                .Where(m => m.UserId == userId)
                .Select(m => m.OrganizationId)
                .Distinct()
                .ToList();

            return _organizations
                .Where(o => userOrgIds.Contains(o.Id))
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        public async Task<Organization?> GetOrganizationByIdAsync(string organizationId)
        {
            await Task.Delay(100);
            return _organizations.FirstOrDefault(o => o.Id == organizationId);
        }

        public async Task<List<User>> GetOrganizationEmployeesAsync(string organizationId)
        {
            await Task.Delay(100);
            var employeeIds = _members
                .Where(m => m.OrganizationId == organizationId && m.Role == "Employee")
                .Select(m => m.UserId)
                .ToList();

            // В реальном приложении здесь был бы запрос к сервису пользователей
            // Для демонстрации возвращаем пустой список
            return new List<User>();
        }

        public async Task<bool> RemoveEmployeeFromOrganizationAsync(string organizationId, string employeeId)
        {
            await Task.Delay(500);

            var member = _members.FirstOrDefault(m =>
                m.OrganizationId == organizationId && m.UserId == employeeId && m.Role == "Employee");

            if (member != null)
            {
                _members.Remove(member);

                var organization = _organizations.FirstOrDefault(o => o.Id == organizationId);
                if (organization != null)
                {
                    organization.EmployeeIds.Remove(employeeId);
                }

                return true;
            }
            return false;
        }

        public async Task<bool> IsUserOrganizationOwnerAsync(string organizationId, string userId)
        {
            await Task.Delay(100);
            return _members.Any(m =>
                m.OrganizationId == organizationId &&
                m.UserId == userId &&
                m.Role == "Owner");
        }

        public async Task<bool> UpdateOrganizationAsync(Organization organization)
        {
            await Task.Delay(500);

            var existingOrganization = _organizations.FirstOrDefault(o => o.Id == organization.Id);
            if (existingOrganization != null)
            {
                existingOrganization.Name = organization.Name;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteOrganizationAsync(string organizationId, string ownerId)
        {
            await Task.Delay(500);

            // Проверяем, что пользователь является владельцем
            var isOwner = await IsUserOrganizationOwnerAsync(organizationId, ownerId);
            if (!isOwner)
                return false;

            var organization = _organizations.FirstOrDefault(o => o.Id == organizationId);
            if (organization != null)
            {
                // Удаляем все связи членов с этой организацией
                var membersToRemove = _members.Where(m => m.OrganizationId == organizationId).ToList();
                foreach (var member in membersToRemove)
                {
                    _members.Remove(member);
                }

                // Удаляем организацию
                _organizations.Remove(organization);
                return true;
            }
            return false;
        }

        public async Task<List<OrganizationMember>> GetOrganizationMembersAsync(string organizationId)
        {
            await Task.Delay(100);
            return _members
                .Where(m => m.OrganizationId == organizationId)
                .ToList();
        }
    }
}