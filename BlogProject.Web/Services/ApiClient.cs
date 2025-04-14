using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace BlogProject.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);

            // Default Headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Session'dan token'ı al ve ekle
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Generic GET method with error handling
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                // 401 Unauthorized durumunda
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Session'ı temizle
                    _httpContextAccessor.HttpContext.Session.Clear();
                    throw new UnauthorizedAccessException("API erişim yetkisi reddedildi. Lütfen tekrar giriş yapın.");
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                throw;
            }
        }

        // POST method for application/json content
        public async Task<T> PostAsync<T, R>(string endpoint, R data)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext.Session.Clear();
                    throw new UnauthorizedAccessException("API erişim yetkisi reddedildi. Lütfen tekrar giriş yapın.");
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}");
            }
            catch
            {
                throw;
            }
        }

        // POST method for multipart/form-data content
        public async Task<T> PostFormAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                // API çağrısını yapalım ve yanıtı alalım
                var response = await _httpClient.PostAsync(endpoint, formData);

                // Yanıt içeriğini alalım (hata mesajı için)
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(object) && string.IsNullOrEmpty(responseContent))
                        return default;

                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                // Hata durumunda yanıt içeriğini loglayalım
                Console.WriteLine($"API Hatası: {response.StatusCode}, Yanıt İçeriği: {responseContent}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext.Session.Clear();
                    throw new UnauthorizedAccessException("API erişim yetkisi reddedildi. Lütfen tekrar giriş yapın.");
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}, Yanıt İçeriği: {responseContent}");
            }
            catch (Exception ex)
            {
                // Hata detaylarını konsola yazdıralım
                Console.WriteLine($"API çağrısı sırasında hata: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");

                throw;
            }
        }

        // PUT method for application/json content
        public async Task<T> PutAsync<T, R>(string endpoint, R data)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext.Session.Clear();
                    throw new UnauthorizedAccessException("API erişim yetkisi reddedildi. Lütfen tekrar giriş yapın.");
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}");
            }
            catch
            {
                throw;
            }
        }

        // PUT method for multipart/form-data content
        public async Task<T> PutFormAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                var response = await _httpClient.PutAsync(endpoint, formData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext.Session.Clear();
                    throw new UnauthorizedAccessException("API erişim yetkisi reddedildi. Lütfen tekrar giriş yapın.");
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}");
            }
            catch
            {
                throw;
            }
        }

        // DELETE method
        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _httpContextAccessor.HttpContext.Session.Clear();
                    throw new UnauthorizedAccessException("API erişim yetkisi reddedildi. Lütfen tekrar giriş yapın.");
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}");
            }
            catch
            {
                throw;
            }
        }

        // Token ayarlamak için method
        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Token'ı kaldırmak için method
        public void RemoveAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}