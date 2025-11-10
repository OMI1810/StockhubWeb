using StockhubWeb.Models;

namespace StockhubWeb.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly List<Organization> _organizations = new();
        private readonly List<OrganizationMember> _members = new();

        public async Task<Organization?> GetUserOrganizationAsync(string userId)
        {
            await Task.Delay(100);
            var member = _members.FirstOrDefault(m => m.UserId == userId);
            return member != null ? _organizations.FirstOrDefault(o => o.Id == member.OrganizationId) : null;
        }

        public async Task<bool> CreateOrganizationAsync(string name, string password, string ownerId)
        {
            await Task.Delay(500);

            var organization = new Organization
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                OrganizationPassword = password,
                OwnerId = ownerId,
                CreatedAt = DateTime.Now
            };

            _organizations.Add(organization);
            _members.Add(new OrganizationMember
            {
                UserId = ownerId,
                OrganizationId = organization.Id,
                Role = "Owner"
            });

            return true;
        }

        public async Task<bool> JoinOrganizationAsync(string organizationId, string password, string userId)
        {
            await Task.Delay(500);

            var organization = _organizations.FirstOrDefault(o => o.Id == organizationId && o.OrganizationPassword == password);
            if (organization != null)
            {
                _members.Add(new OrganizationMember
                {
                    UserId = userId,
                    OrganizationId = organizationId,
                    Role = "Employee"
                });
                return true;
            }
            return false;
        }

        public async Task<List<Organization>> GetUserOrganizationsAsync(string userId)
        {
            await Task.Delay(100);
            var userOrgIds = _members.Where(m => m.UserId == userId).Select(m => m.OrganizationId);
            return _organizations.Where(o => userOrgIds.Contains(o.Id)).ToList();
        }
    }
}