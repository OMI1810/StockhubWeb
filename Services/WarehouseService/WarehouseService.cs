using StockhubWeb.Models;

namespace StockhubWeb.Services.WarehouseService
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseApiService _warehouseApiService;
        private readonly List<Warehouse> _warehouses = new();

        public WarehouseService(IWarehouseApiService warehouseApiService)
        {
            _warehouseApiService = warehouseApiService;
        }

        public async Task<List<Warehouse>> GetOrganizationWarehousesAsync()
        {
            var result = await _warehouseApiService.GetOrganizationWarehousesAsync();
            return result.Success ? result.Data ?? new List<Warehouse>() : new List<Warehouse>();
        }

        public async Task<Warehouse?> CreateWarehouseAsync(CreateWarehouseModel model)
        {
            var result = await _warehouseApiService.CreateWarehouseAsync(model);
            return result.Success ? result.Data : null;
        }
    }
}