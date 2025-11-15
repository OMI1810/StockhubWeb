namespace StockhubWeb.Models
{
    public class Organization
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class CreateOrganizationModel
    {
        public string Name { get; set; } = string.Empty;
    }

    public class OrganizationResponse
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Role { get; set; }
    }

    public class SelectOrganizationResponse
    {
        public string Message { get; set; } = string.Empty;
        public Organization? Organization { get; set; }
    }

    public class CurrentOrganizationResponse
    {
        public string? OrganizationId { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
    }
}