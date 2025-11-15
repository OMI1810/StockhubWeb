using StockhubWeb.Models;

namespace StockhubWeb.Services.WarehouseService
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetOrganizationWarehousesAsync();
        Task<Warehouse?> CreateWarehouseAsync(CreateWarehouseModel model);
    }
}