using System.ComponentModel.DataAnnotations;

namespace stockhub_web.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? MiddleName { get; set; }
        public UserRole Role { get; set; }
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }

    public enum UserRole
    {
        Employee,
        Organizer
    }
}