namespace StockhubWeb.Models
{
    public class InvoiceData
    {
        public Warehouse Warehouse { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public DateTime GenerationDate { get; set; } = DateTime.Now;
        public string InvoiceNumber { get; set; } = string.Empty;

        public int TotalQuantity => Products.Sum(p => p.Quantity);
        public decimal TotalValue => Products.Sum(p => p.Quantity * p.Price);
    }
}
