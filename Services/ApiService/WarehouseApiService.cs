// Services/WarehouseApiService.cs
using StockhubWeb.Models;
using System.Text;
using System.Text.Json;

namespace StockhubWeb.Services
{
    public interface IWarehouseApiService
    {
        Task<ApiResponse<Warehouse>> CreateWarehouseAsync(CreateWarehouseModel model);
        Task<ApiResponse<List<Warehouse>>> GetOrganizationWarehousesAsync();
    }

    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public WarehouseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:4000");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<Warehouse>> CreateWarehouseAsync(CreateWarehouseModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/warehouses", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<Warehouse>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Warehouse>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<List<Warehouse>>> GetOrganizationWarehousesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/warehouses");
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<List<Warehouse>>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Warehouse>>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response, string responseContent) where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                return new ApiResponse<T>
                {
                    Success = true,
                    Data = data,
                    StatusCode = (int)response.StatusCode
                };
            }
            else
            {
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = errorResponse?.Message,
                        Errors = errorResponse?.Errors ?? new List<string>(),
                        StatusCode = (int)response.StatusCode
                    };
                }
                catch
                {
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode}",
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
        }
    }
}