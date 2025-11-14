// User.cs
namespace StockhubWeb.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool HasMiddleName { get; set; } = true;
        public UserRole Role { get; set; } = UserRole.Employee;
        public string? OrganizationId { get; set; }
    }

    public enum UserRole
    {
        Employee,
        Organizer
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Employee;
    }

    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public bool HasMiddleName { get; set; } = true;
        public UserRole Role { get; set; } = UserRole.Organizer;
    }

    public class InviteEmployeeModel
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string OrganizationId { get; set; } = string.Empty;
        public string? WarehouseId { get; set; }
    }
}