namespace stockhub_web.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}
