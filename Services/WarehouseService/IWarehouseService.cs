using StockhubWeb.Models;

namespace StockhubWeb.Services.WarehouseService
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetOrganizationWarehousesAsync(string organizationId);
        Task<Warehouse?> GetWarehouseAsync(string warehouseId);
        Task<bool> CreateWarehouseAsync(Warehouse warehouse);
        Task<bool> UpdateWarehouseAsync(Warehouse warehouse);
        Task<List<Product>> GetWarehouseProductsAsync(string warehouseId);
        Task<bool> AddProductAsync(Product product);
        Task<InvoiceData> CreateInvoiceAsync(string warehouseId, List<Product> products, string invoiceNumber);
        Task<InvoiceData> CreateTransferInvoiceAsync(string fromWarehouseId, string toWarehouseId, Product product, int quantity, string notes);
    }

    public class ProductTransfer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string FromWarehouseId { get; set; } = string.Empty;
        public string FromWarehouseName { get; set; } = string.Empty;
        public string ToWarehouseId { get; set; } = string.Empty;
        public string ToWarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
    }
}