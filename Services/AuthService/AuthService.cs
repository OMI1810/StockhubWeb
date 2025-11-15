using StockhubWeb.Models;

namespace StockhubWeb.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private User? _currentUser;
        private readonly IAuthApiService _authApiService;

        public AuthService(IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginModel loginModel)
        {
            var apiResponse = await _authApiService.LoginAsync(loginModel);

            if (apiResponse.Success && apiResponse.Data?.User != null)
            {
                _currentUser = apiResponse.Data.User;
            }

            return apiResponse;
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterModel registerModel)
        {
            return await _authApiService.RegisterAsync(registerModel);
        }

        public async Task<ApiResponse<AuthResponse>> ConfirmEmailAsync(EmailConfirmationModel confirmationModel)
        {
            var apiResponse = await _authApiService.ConfirmEmailAsync(confirmationModel);

            if (apiResponse.Success && apiResponse.Data?.User != null)
            {
                _currentUser = apiResponse.Data.User;
            }

            return apiResponse;
        }

        public Task<User?> GetCurrentUserAsync() => Task.FromResult(_currentUser);

        public Task<bool> IsAuthenticatedAsync() => Task.FromResult(_currentUser != null);

        public Task LogoutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }

        public Task SetCurrentUserAsync(User user)
        {
            _currentUser = user;
            return Task.CompletedTask;
        }
    }
}