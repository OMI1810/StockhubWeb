using StockhubWeb.Models;

namespace StockhubWeb.Services
{
    public class AuthService : IAuthService
    {
        private User? _currentUser;
        private readonly List<User> _users = new();
        private readonly IOrganizationService _organizationService;

        public AuthService(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        public async Task<bool> LoginUserAsync(LoginModel request)
        {
            await Task.Delay(500);

            var user = _users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            if (user != null)
            {
                _currentUser = user;
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterUserAsync(RegisterModel request)
        {
            await Task.Delay(500);

            if (_users.Any(u => u.Email == request.Email))
                return false;

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                HasMiddleName = request.HasMiddleName
            };

            _users.Add(user);
            _currentUser = user; // Важно: устанавливаем текущего пользователя

            // Автоматически создаем организацию для нового пользователя
            var orgName = $"{user.FirstName} {user.LastName}";
            await _organizationService.CreateOrganizationAsync(orgName, "default123", user.Id);

            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            await Task.Delay(500);
            return _users.Any(u => u.Email == email);
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            await Task.Delay(500);

            var user = _users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.Password = newPassword;
                return true;
            }
            return false;
        }

        public Task<User?> GetCurrentUserAsync() => Task.FromResult(_currentUser);

        public Task<bool> IsAuthenticatedAsync() => Task.FromResult(_currentUser != null);

        public Task LogoutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }
    }
}