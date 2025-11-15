// Services/AuthApiService.cs
using System.Net;
using System.Text;
using System.Text.Json;
using StockhubWeb.Models;

namespace StockhubWeb.Services
{
    public interface IAuthApiService
    {
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterModel registerModel);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginModel loginModel);
        Task<ApiResponse<AuthResponse>> ConfirmEmailAsync(EmailConfirmationModel confirmationModel);
    }

    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:4000";
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                var json = JsonSerializer.Serialize(registerModel, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<AuthResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginModel, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<AuthResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "Ошибка соединения с сервером",
                    Errors = new List<string> { ex.Message },
                    StatusCode = 500
                };
            }
        }

        public async Task<ApiResponse<AuthResponse>> ConfirmEmailAsync(EmailConfirmationModel confirmationModel)
        {
            try
            {
                var json = JsonSerializer.Serialize(confirmationModel, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/auth/email-confirmation", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return await HandleResponse<AuthResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse>
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