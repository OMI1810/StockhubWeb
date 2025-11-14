using StockhubWeb.Models;

namespace StockhubWeb.Services.WarehouseService
{
    public class WarehouseService : IWarehouseService
    {
        private readonly List<Warehouse> _warehouses = new();
        private readonly List<Product> _products = new();
        private readonly List<ProductTransfer> _transfers = new();

        public async Task<bool> CreateProductTransferAsync(ProductTransfer transfer)
        {
            await Task.Delay(500);

            // В реальном приложении здесь была бы логика перемещения товара
            // между складами и обновления количеств

            _transfers.Add(transfer);
            return true;
        }

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

        public async Task<List<ProductTransfer>> GetProductTransfersAsync(string? warehouseId = null)
        {
            await Task.Delay(100);
            if (string.IsNullOrEmpty(warehouseId))
            {
                return _transfers.OrderByDescending(t => t.TransferDate).ToList();
            }
            return _transfers
                .Where(t => t.FromWarehouseId == warehouseId || t.ToWarehouseId == warehouseId)
                .OrderByDescending(t => t.TransferDate)
                .ToList();
        }

        public async Task<bool> CreateTestWarehousesAsync(string organizationId)
        {
            await Task.Delay(500);

            // Создаем базовые склады для организации с тестовыми товарами
            var warehouses = new[]
            {
                new Warehouse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Основной склад",
                    OrganizationId = organizationId,
                    IsActive = true
                },
                new Warehouse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Резервный склад",
                    OrganizationId = organizationId,
                    IsActive = true
                },
                new Warehouse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Региональный склад",
                    OrganizationId = organizationId,
                    IsActive = true
                },
                new Warehouse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Архив",
                    OrganizationId = organizationId,
                    IsActive = false
                }
            };

            foreach (var warehouse in warehouses)
            {
                _warehouses.Add(warehouse);
            }

            // Создаем несколько тестовых товаров для новой организации
            var testProducts = new List<Product>
            {
                new Product {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Ноутбук бизнес-класса",
                    Description = "Корпоративный ноутбук для сотрудников",
                    Category = "Электроника",
                    Price = 75990,
                    Quantity = 5,
                    WarehouseId = warehouses[0].Id,
                    CreatedAt = DateTime.Now
                },
                new Product {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Офисная мышь",
                    Description = "Беспроводная мышь для офиса",
                    Category = "Периферия",
                    Price = 2490,
                    Quantity = 20,
                    WarehouseId = warehouses[0].Id,
                    CreatedAt = DateTime.Now
                },
                new Product {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Монитор 24\"",
                    Description = "Офисный монитор Full HD",
                    Category = "Мониторы",
                    Price = 18990,
                    Quantity = 8,
                    WarehouseId = warehouses[1].Id,
                    CreatedAt = DateTime.Now
                },
                new Product {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Сетевое оборудование",
                    Description = "Коммутатор 24 порта",
                    Category = "Сетевое оборудование",
                    Price = 45900,
                    Quantity = 2,
                    WarehouseId = warehouses[2].Id,
                    CreatedAt = DateTime.Now
                }
            };

            _products.AddRange(testProducts);

            // Создаем тестовые перемещения для новой организации
            var testTransfers = new List<ProductTransfer>
            {
                new ProductTransfer
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = testProducts[2].Id,
                    ProductName = testProducts[2].Name,
                    FromWarehouseId = warehouses[0].Id,
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = warehouses[1].Id,
                    ToWarehouseName = "Резервный склад",
                    Quantity = 3,
                    TransferDate = DateTime.Now.AddDays(-2),
                    Notes = "Первоначальное распределение запасов"
                },
                new ProductTransfer
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = testProducts[3].Id,
                    ProductName = testProducts[3].Name,
                    FromWarehouseId = warehouses[1].Id,
                    FromWarehouseName = "Резервный склад",
                    ToWarehouseId = warehouses[2].Id,
                    ToWarehouseName = "Региональный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-1),
                    Notes = "Перемещение для установки в филиале"
                }
            };

            _transfers.AddRange(testTransfers);

            return true;
        }
    }
}