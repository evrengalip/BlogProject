using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
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

        public async Task<T> PostAsync<T, R>(string endpoint, R data)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
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

        public async Task<T> PostFormAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, formData);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(object) && string.IsNullOrEmpty(responseContent))
                        return default;

                    return JsonConvert.DeserializeObject<T>(responseContent);
                }

                throw new HttpRequestException($"API Hatası: {response.StatusCode}, Yanıt: {responseContent}");
            }
            catch
            {
                throw;
            }
        }

        public async Task<T> PutAsync<T, R>(string endpoint, R data)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
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

        public async Task<T> PutFormAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                var response = await _httpClient.PutAsync(endpoint, formData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
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

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
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

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void RemoveAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
