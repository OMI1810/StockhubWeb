namespace StockhubWeb.Models
{
    public class User
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Role { get; set; } = "USER";
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordRepeat { get; set; } = string.Empty;
    }

    public class EmailConfirmationModel
    {
        public string Token { get; set; } = string.Empty;
    }

    public class InviteEmployeeModel
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string OrganizationId { get; set; } = string.Empty;
    }

    public enum UserRole
    {
        ADMIN,
        REGULAR
    }
}