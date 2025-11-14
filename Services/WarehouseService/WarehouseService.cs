using StockhubWeb.Models;

namespace StockhubWeb.Services.WarehouseService
{
    public class WarehouseService : IWarehouseService
    {
        private readonly List<Warehouse> _warehouses = new();
        private readonly List<Product> _products = new();
        private readonly List<ProductTransfer> _transfers = new();

        public async Task<List<Warehouse>> GetOrganizationWarehousesAsync(string organizationId)
        {
            await Task.Delay(100);
            var warehouses = _warehouses.Where(w => w.OrganizationId == organizationId).ToList();

            foreach (var warehouse in warehouses)
            {
                warehouse.Products = _products.Where(p => p.WarehouseId == warehouse.Id).ToList();
            }

            return warehouses;
        }

        public async Task<Warehouse?> GetWarehouseAsync(string warehouseId)
        {
            await Task.Delay(100);
            var warehouse = _warehouses.FirstOrDefault(w => w.Id == warehouseId);
            if (warehouse != null)
            {
                warehouse.Products = _products.Where(p => p.WarehouseId == warehouseId).ToList();
            }
            return warehouse;
        }

        public async Task<bool> CreateWarehouseAsync(Warehouse warehouse)
        {
            await Task.Delay(500);
            _warehouses.Add(warehouse);
            return true;
        }

        public async Task<bool> UpdateWarehouseAsync(Warehouse warehouse)
        {
            await Task.Delay(500);
            var existing = _warehouses.FirstOrDefault(w => w.Id == warehouse.Id);
            if (existing != null)
            {
                existing.Name = warehouse.Name;
                existing.IsActive = warehouse.IsActive;
                return true;
            }
            return false;
        }

        public async Task<List<Product>> GetWarehouseProductsAsync(string warehouseId)
        {
            await Task.Delay(100);
            return _products.Where(p => p.WarehouseId == warehouseId).ToList();
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            await Task.Delay(500);
            _products.Add(product);
            return true;
        }


        public async Task<InvoiceData> CreateInvoiceAsync(string warehouseId, List<Product> products, string invoiceNumber)
        {
            await Task.Delay(100);

            var warehouse = await GetWarehouseAsync(warehouseId);
            if (warehouse == null)
                throw new ArgumentException("Склад не найден");

            return new InvoiceData
            {
                Warehouse = warehouse,
                Products = products,
                InvoiceNumber = invoiceNumber,
                GenerationDate = DateTime.Now
            };
        }

        public async Task<InvoiceData> CreateTransferInvoiceAsync(string fromWarehouseId, string toWarehouseId, Product product, int quantity, string notes)
        {
            await Task.Delay(100);

            var fromWarehouse = await GetWarehouseAsync(fromWarehouseId);
            var toWarehouse = await GetWarehouseAsync(toWarehouseId);

            if (fromWarehouse == null || toWarehouse == null)
                throw new ArgumentException("Склады не найдены");

            var transferProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price,
                Quantity = quantity,
                WarehouseId = toWarehouseId
            };

            return new InvoiceData
            {
                Warehouse = fromWarehouse,
                Products = new List<Product> { transferProduct },
                InvoiceNumber = await GenerateTransferInvoiceNumberAsync(),
                GenerationDate = DateTime.Now
            };
        }

        private async Task<string> GenerateTransferInvoiceNumberAsync()
        {
            await Task.Delay(50);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"TRF-{timestamp}";
        }
    }
}