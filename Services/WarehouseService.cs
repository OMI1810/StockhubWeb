using StockhubWeb.Models;

namespace StockhubWeb.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly List<Warehouse> _warehouses = new();
        private readonly List<Product> _products = new();
        private readonly List<ProductTransfer> _transfers = new();
        private bool _testDataCreated = false;

        public WarehouseService()
        {
            CreateTestData();
        }

        private void CreateTestData()
        {
            if (_testDataCreated) return;

            // Создаем тестовые склады
            var mainWarehouse = new Warehouse
            {
                Id = "warehouse-1",
                Name = "Основной склад",
                OrganizationId = "test-org",
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-30)
            };

            var reserveWarehouse = new Warehouse
            {
                Id = "warehouse-2",
                Name = "Резервный склад",
                OrganizationId = "test-org",
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-20)
            };

            var regionalWarehouse = new Warehouse
            {
                Id = "warehouse-3",
                Name = "Региональный склад",
                OrganizationId = "test-org",
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-15)
            };

            var archiveWarehouse = new Warehouse
            {
                Id = "warehouse-4",
                Name = "Архивный склад",
                OrganizationId = "test-org",
                IsActive = false,
                CreatedAt = DateTime.Now.AddDays(-10)
            };

            _warehouses.AddRange(new[] { mainWarehouse, reserveWarehouse, regionalWarehouse, archiveWarehouse });

            // Создаем тестовые товары с историей перемещений
            var products = new List<Product>
            {
                // Существующие товары...
                // ... (предыдущие товары остаются без изменений)

                // НОВЫЙ ТОВАР: который перемещался через все три активных склада
                new Product {
                    Id = "product-13",
                    Name = "Серверное оборудование HP ProLiant",
                    Description = "Серверная стойка DL380 Gen10, 2x Xeon Silver, 64GB RAM",
                    Category = "Серверное оборудование",
                    Price = 325000,
                    Quantity = 1,
                    WarehouseId = "warehouse-3", // Сейчас на региональном складе
                    CreatedAt = DateTime.Now.AddDays(-60),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },

                // Еще один товар с сложной историей перемещений
                new Product {
                    Id = "product-14",
                    Name = "Системный блок для рабочей станции",
                    Description = "Workstation Dell Precision, RTX A4000, 32GB RAM, 1TB SSD",
                    Category = "Компьютеры",
                    Price = 189990,
                    Quantity = 2,
                    WarehouseId = "warehouse-2", // Сейчас на резервном складе
                    CreatedAt = DateTime.Now.AddDays(-45),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                }
            };

            // Добавляем новые товары к существующим
            _products.AddRange(products);

            // Создаем расширенную историю перемещений
            var transfers = new List<ProductTransfer>
            {
                // Существующие перемещения...
                // ... (предыдущие перемещения остаются)

                // ЦЕПОЧКА ПЕРЕМЕЩЕНИЙ: Серверное оборудование через все три склада
                new ProductTransfer
                {
                    Id = "transfer-16",
                    ProductId = "product-13",
                    ProductName = "Серверное оборудование HP ProLiant",
                    FromWarehouseId = "warehouse-1",
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = "warehouse-2",
                    ToWarehouseName = "Резервный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-40),
                    Notes = "Первоначальное перемещение на резерв для тестирования"
                },
                new ProductTransfer
                {
                    Id = "transfer-17",
                    ProductId = "product-13",
                    ProductName = "Серверное оборудование HP ProLiant",
                    FromWarehouseId = "warehouse-2",
                    FromWarehouseName = "Резервный склад",
                    ToWarehouseId = "warehouse-3",
                    ToWarehouseName = "Региональный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-20),
                    Notes = "Перемещение в региональный филиал для установки"
                },
                new ProductTransfer
                {
                    Id = "transfer-18",
                    ProductId = "product-13",
                    ProductName = "Серверное оборудование HP ProLiant",
                    FromWarehouseId = "warehouse-3",
                    FromWarehouseName = "Региональный склад",
                    ToWarehouseId = "warehouse-1",
                    ToWarehouseName = "Основной склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-5),
                    Notes = "Возврат для обслуживания и апгрейда"
                },
                new ProductTransfer
                {
                    Id = "transfer-19",
                    ProductId = "product-13",
                    ProductName = "Серверное оборудование HP ProLiant",
                    FromWarehouseId = "warehouse-1",
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = "warehouse-3",
                    ToWarehouseName = "Региональный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-1),
                    Notes = "Повторное перемещение после обслуживания"
                },

                // СЛОЖНАЯ ЦЕПОЧКА: Системный блок с перемещениями между всеми складами
                new ProductTransfer
                {
                    Id = "transfer-20",
                    ProductId = "product-14",
                    ProductName = "Системный блок для рабочей станции",
                    FromWarehouseId = "warehouse-1",
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = "warehouse-2",
                    ToWarehouseName = "Резервный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-35),
                    Notes = "Резервное хранение"
                },
                new ProductTransfer
                {
                    Id = "transfer-21",
                    ProductId = "product-14",
                    ProductName = "Системный блок для рабочей станции",
                    FromWarehouseId = "warehouse-2",
                    FromWarehouseName = "Резервный склад",
                    ToWarehouseId = "warehouse-3",
                    ToWarehouseName = "Региональный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-25),
                    Notes = "Перемещение для демонстрации клиенту"
                },
                new ProductTransfer
                {
                    Id = "transfer-22",
                    ProductId = "product-14",
                    ProductName = "Системный блок для рабочей станции",
                    FromWarehouseId = "warehouse-3",
                    FromWarehouseName = "Региональный склад",
                    ToWarehouseId = "warehouse-1",
                    ToWarehouseName = "Основной склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-15),
                    Notes = "Возврат после демонстрации"
                },
                new ProductTransfer
                {
                    Id = "transfer-23",
                    ProductId = "product-14",
                    ProductName = "Системный блок для рабочей станции",
                    FromWarehouseId = "warehouse-1",
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = "warehouse-2",
                    ToWarehouseName = "Резервный склад",
                    Quantity = 2, // Вторая единица товара
                    TransferDate = DateTime.Now.AddDays(-10),
                    Notes = "Комплектное перемещение на резерв"
                },
                new ProductTransfer
                {
                    Id = "transfer-24",
                    ProductId = "product-14",
                    ProductName = "Системный блок для рабочей станции",
                    FromWarehouseId = "warehouse-2",
                    FromWarehouseName = "Резервный склад",
                    ToWarehouseId = "warehouse-3",
                    ToWarehouseName = "Региональный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-7),
                    Notes = "Частичное перемещение для выполнения заказа"
                },
                new ProductTransfer
                {
                    Id = "transfer-25",
                    ProductId = "product-14",
                    ProductName = "Системный блок для рабочей станции",
                    FromWarehouseId = "warehouse-3",
                    FromWarehouseName = "Региональный склад",
                    ToWarehouseId = "warehouse-2",
                    ToWarehouseName = "Резервный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-3),
                    Notes = "Корректировка запасов между складами"
                },

                // ДОПОЛНИТЕЛЬНО: Товар, который побывал во всех складах включая архив
                new ProductTransfer
                {
                    Id = "transfer-26",
                    ProductId = "product-8", // Ноутбук HP (уже в архиве)
                    ProductName = "Ноутбук HP EliteBook 840 G5",
                    FromWarehouseId = "warehouse-1",
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = "warehouse-2",
                    ToWarehouseName = "Резервный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-50),
                    Notes = "Первоначальное перемещение на резерв"
                },
                new ProductTransfer
                {
                    Id = "transfer-27",
                    ProductId = "product-8",
                    ProductName = "Ноутбук HP EliteBook 840 G5",
                    FromWarehouseId = "warehouse-2",
                    FromWarehouseName = "Резервный склад",
                    ToWarehouseId = "warehouse-3",
                    ToWarehouseName = "Региональный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-40),
                    Notes = "Использование в региональном офисе"
                },
                new ProductTransfer
                {
                    Id = "transfer-28",
                    ProductId = "product-8",
                    ProductName = "Ноутбук HP EliteBook 840 G5",
                    FromWarehouseId = "warehouse-3",
                    FromWarehouseName = "Региональный склад",
                    ToWarehouseId = "warehouse-1",
                    ToWarehouseName = "Основной склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-30),
                    Notes = "Возврат для оценки состояния"
                },
                new ProductTransfer
                {
                    Id = "transfer-29",
                    ProductId = "product-8",
                    ProductName = "Ноутбук HP EliteBook 840 G5",
                    FromWarehouseId = "warehouse-1",
                    FromWarehouseName = "Основной склад",
                    ToWarehouseId = "warehouse-4",
                    ToWarehouseName = "Архивный склад",
                    Quantity = 1,
                    TransferDate = DateTime.Now.AddDays(-20),
                    Notes = "Перемещение в архив как устаревшее оборудование"
                }
            };

            // Добавляем новые перемещения к существующим
            _transfers.AddRange(transfers);

            // Обновляем склады с товарами
            UpdateWarehouseProducts();

            _testDataCreated = true;
        }

        private void UpdateWarehouseProducts()
        {
            foreach (var warehouse in _warehouses)
            {
                warehouse.Products = _products.Where(p => p.WarehouseId == warehouse.Id).ToList();
            }
        }

        // Остальные методы остаются без изменений...
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