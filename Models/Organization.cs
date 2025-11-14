namespace StockhubWeb.Models
{
    public class Organization
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<string> EmployeeIds { get; set; } = new();
    }

    public class OrganizationMember
    {
        public string UserId { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
    }
}