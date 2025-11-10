namespace stockhub-web.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganizationPassword { get; set; }
        public int OrganizerId { get; set; }
        public User Organizer { get; set; }
        public List<User> Employees { get; set; } = new();
        public List<Warehouse> Warehouses { get; set; } = new();
    }
}