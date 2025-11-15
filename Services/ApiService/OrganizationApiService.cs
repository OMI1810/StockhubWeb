// Services/OrganizationApiService.cs
using StockhubWeb.Models;
using System.Text;
using System.Text.Json;

namespace StockhubWeb.Services
{
    public interface IOrganizationApiService
    {
        Task<ApiResponse<List<Organization>>> GetUserOrganizationsAsync();
        Task<ApiResponse<Organization>> CreateOrganizationAsync(CreateOrganizationModel model);
        Task<ApiResponse<SelectOrganizationResponse>> SelectOrganizationAsync(string organizationId);
        Task<ApiResponse<CurrentOrganizationResponse>> GetCurrentOrganizationAsync();
        Task<ApiResponse<string>> DeleteOrganizationAsync(string organizationId);
    }

    public class OrganizationApiService : IOrganizationApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrganizationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:4000");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<List<Organization>>> GetUserOrganizationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/organizations");
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<List<Organization>>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Organization>>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<Organization>> CreateOrganizationAsync(CreateOrganizationModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/organizations", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<Organization>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Organization>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<SelectOrganizationResponse>> SelectOrganizationAsync(string organizationId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/organizations/{organizationId}/select", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<SelectOrganizationResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SelectOrganizationResponse>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<CurrentOrganizationResponse>> GetCurrentOrganizationAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/organizations/current");
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<CurrentOrganizationResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CurrentOrganizationResponse>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteOrganizationAsync(string organizationId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/organizations/{organizationId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent, _jsonOptions);
                    return new ApiResponse<string>
                    {
                        Success = true,
                        Data = result?["message"],
                        StatusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    return await HandleErrorResponse<string>(response, responseContent);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
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
                return await HandleErrorResponse<T>(response, responseContent);
            }
        }

        private async Task<ApiResponse<T>> HandleErrorResponse<T>(HttpResponseMessage response, string responseContent) where T : class
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);

                var apiResponse = new ApiResponse<T>
                {
                    Success = false,
                    Message = errorResponse?.Message,
                    StatusCode = (int)response.StatusCode
                };

                if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                {
                    apiResponse.Errors = errorResponse.Errors;
                }
                else if (!string.IsNullOrEmpty(errorResponse?.Message))
                {
                    apiResponse.Errors = new List<string> { errorResponse.Message };
                }

                return apiResponse;
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