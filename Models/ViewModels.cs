using System.ComponentModel.DataAnnotations;

namespace stockhub-web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Display(Name = "Отчество")]
        public string? MiddleName { get; set; }

        public bool NoMiddleName { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }

    public class OrganizationViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string OrganizationPassword { get; set; }
    }

    public class WarehouseViewModel
    {
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class InviteEmployeeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}