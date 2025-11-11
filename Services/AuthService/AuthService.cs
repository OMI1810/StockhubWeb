using StockhubWeb.Models;
using StockhubWeb.Services.OrganizationService;

namespace StockhubWeb.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private User? _currentUser;
        private readonly List<User> _users = new();
        private readonly IOrganizationService _organizationService;

        public AuthService(IOrganizationService organizationService)
        {
            _organizationService = organizationService;

            // Создаем тестового организатора для демонстрации
            CreateTestOrganizer();
            // Создаем тестового сотрудника для демонстрации
            CreateTestEmployee();
        }

        private void CreateTestOrganizer()
        {
            var organizer = new User
            {
                Email = "organizer@example.com",
                Password = "123456",
                FirstName = "Иван",
                LastName = "Петров",
                Role = UserRole.Organizer
            };
            _users.Add(organizer);

            // Создаем организацию для тестового организатора
            _organizationService.CreateOrganizationAsync("Тестовая организация", "default123", organizer.Id);
        }

        private void CreateTestEmployee()
        {
            var employee = new User
            {
                Email = "employee@example.com",
                Password = "123456",
                FirstName = "Мария",
                LastName = "Сидорова",
                Role = UserRole.Employee,
                OrganizationId = _users.First(u => u.Role == UserRole.Organizer).Id
            };
            _users.Add(employee);
        }

        public async Task<bool> LoginUserAsync(LoginModel request)
        {
            await Task.Delay(500);

            var user = _users.FirstOrDefault(u =>
                u.Email == request.Email &&
                u.Password == request.Password);

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

            // При регистрации пользователь всегда становится организатором
            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                HasMiddleName = request.HasMiddleName,
                Role = UserRole.Organizer // Всегда организатор
            };

            _users.Add(user);
            _currentUser = user;

            // Автоматически создаем организацию для нового организатора
            var orgName = $"{user.FirstName} {user.LastName}";
            await _organizationService.CreateOrganizationAsync(orgName, "default123", user.Id);

            return true;
        }

        public async Task<bool> InviteEmployeeAsync(InviteEmployeeModel request)
        {
            await Task.Delay(500);

            if (_users.Any(u => u.Email == request.Email))
                return false;

            // Генерируем случайный пароль
            var password = GenerateRandomPassword();

            var employee = new User
            {
                Email = request.Email,
                Password = password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Role = UserRole.Employee,
                OrganizationId = request.OrganizationId
            };

            _users.Add(employee);

            // Добавляем сотрудника в организацию
            await _organizationService.AddEmployeeToOrganizationAsync(request.OrganizationId, employee.Id);

            // В реальном приложении здесь была бы отправка email с паролем
            Console.WriteLine($"Сотрудник {request.Email} приглашен. Пароль: {password}");

            return true;
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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

        public async Task<List<User>> GetOrganizationEmployeesAsync(string organizationId)
        {
            await Task.Delay(100);
            return _users.Where(u => u.OrganizationId == organizationId && u.Role == UserRole.Employee).ToList();
        }

        public async Task<bool> RemoveEmployeeAsync(string employeeId)
        {
            await Task.Delay(500);

            var employee = _users.FirstOrDefault(u => u.Id == employeeId && u.Role == UserRole.Employee);
            if (employee != null)
            {
                _users.Remove(employee);
                return true;
            }
            return false;
        }
    }
}