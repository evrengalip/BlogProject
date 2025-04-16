using BlogProject.Entity.DTOs.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

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
                    // Token'ı session'da sakla
                    _httpContextAccessor.HttpContext.Session.SetString("JWTToken", response.Token);

                    // Kullanıcı bilgilerini daha güvenli bir şekilde session'da sakla
                    _httpContextAccessor.HttpContext.Session.SetString("UserName", response.User.UserName);
                    _httpContextAccessor.HttpContext.Session.SetString("FirstName", response.User.FirstName);
                    _httpContextAccessor.HttpContext.Session.SetString("LastName", response.User.LastName);
                    _httpContextAccessor.HttpContext.Session.SetString("Email", response.User.Email);
                    _httpContextAccessor.HttpContext.Session.SetString("Roles", string.Join(",", response.User.Roles));
                    _httpContextAccessor.HttpContext.Session.SetString("UserId", response.User.Id.ToString());

                    // API Client'a token'ı ayarla
                    _apiClient.SetAuthToken(response.Token);

                    // Claims daha kapsamlı hazırlanmalı
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, response.User.UserName),
                new Claim(ClaimTypes.Email, response.User.Email),
                new Claim(ClaimTypes.NameIdentifier, response.User.Id.ToString()),
                new Claim(ClaimTypes.GivenName, response.User.FirstName),
                new Claim(ClaimTypes.Surname, response.User.LastName)
            };

                    // Rolleri claim olarak ekle
                    foreach (var role in response.User.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginDto.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30) // Daha uzun süre
                    };

                    await _httpContextAccessor.HttpContext.SignInAsync(
                        "ApplicationCookie",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Loglama eklenebilir
                Console.WriteLine($"Login hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool Succeeded, List<string> Errors)> RegisterAsync(UserRegisterDto registerDto, bool isAdminRegistration = false)
        {
            try
            {
                // Eğer admin kaydı yapılıyorsa, role bilgisini ekleyen özel bir endpoint kullanıyoruz
                string endpoint = isAdminRegistration ? $"{Endpoint}/register-admin" : $"{Endpoint}/register";
                var response = await _apiClient.PostAsync<RegisterResponse, UserRegisterDto>(endpoint, registerDto);
                return (response.Succeeded, response.Errors);
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                return (false, new List<string> { ex.Message });
            }
        }



        public async Task LogoutAsync()
        {
            // Session'ı temizle
            _httpContextAccessor.HttpContext.Session.Clear();

            // API client'dan token'ı kaldır
            _apiClient.RemoveAuthToken();

            // Kullanıcıyı çıkış yaptır
            await _httpContextAccessor.HttpContext.SignOutAsync("ApplicationCookie");
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext.User.IsInRole(role);
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }

        public string GetFullName()
        {
            var firstName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Surname);
            return $"{firstName} {lastName}";
        }

        public Guid GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Guid.Empty;

            return Guid.Parse(userId);
        }

        private class LoginResponse
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
            public UserInfo User { get; set; }
        }

        private class RegisterResponse
        {
            public bool Succeeded { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
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