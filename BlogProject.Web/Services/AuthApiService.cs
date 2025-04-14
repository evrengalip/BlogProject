using BlogProject.Entity.DTOs.Users;
using Microsoft.AspNetCore.Http;

namespace BlogProject.Web.Services
{
    public class AuthApiService
    {
        private readonly ApiClient _apiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string Endpoint = "auth";

        public AuthApiService(ApiClient apiClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var response = await _apiClient.PostAsync<LoginResponse, UserLoginDto>($"{Endpoint}/login", loginDto);

                if (!string.IsNullOrEmpty(response.Token))
                {
                    // Store token in session
                    _httpContextAccessor.HttpContext.Session.SetString("JWTToken", response.Token);

                    // Store user information in session
                    _httpContextAccessor.HttpContext.Session.SetString("UserName", response.User.UserName);
                    _httpContextAccessor.HttpContext.Session.SetString("FirstName", response.User.FirstName);
                    _httpContextAccessor.HttpContext.Session.SetString("LastName", response.User.LastName);
                    _httpContextAccessor.HttpContext.Session.SetString("Email", response.User.Email);
                    _httpContextAccessor.HttpContext.Session.SetString("Roles", string.Join(",", response.User.Roles));

                    // Set token in the API client
                    _apiClient.SetAuthToken(response.Token);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            // Clear session
            _httpContextAccessor.HttpContext.Session.Clear();

            // Remove token from API client
            _apiClient.RemoveAuthToken();
        }

        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("JWTToken"));
        }

        public bool IsInRole(string role)
        {
            var roles = _httpContextAccessor.HttpContext?.Session.GetString("Roles");
            if (string.IsNullOrEmpty(roles))
                return false;

            return roles.Split(',').Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        private class LoginResponse
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
            public UserInfo User { get; set; }
        }

        private class UserInfo
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<string> Roles { get; set; }
        }
    }
}