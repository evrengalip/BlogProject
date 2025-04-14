using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace BlogProject.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public ApiClient(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5002/api/";

            // Set default base address
            _httpClient.BaseAddress = new Uri(_baseUrl);

            // Set default headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add auth token if available
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Generic GET method
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        // Generic POST method for application/json content
        public async Task<T> PostAsync<T, R>(string endpoint, R data)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(endpoint, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        // Generic POST method for multipart/form-data content
        public async Task<T> PostFormAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            var response = await _httpClient.PostAsync(endpoint, formData);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        // Generic PUT method for application/json content
        public async Task<T> PutAsync<T, R>(string endpoint, R data)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync(endpoint, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        // Generic PUT method for multipart/form-data content
        public async Task<T> PutFormAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            var response = await _httpClient.PutAsync(endpoint, formData);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        // Generic DELETE method
        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }

        // Method for setting the authorization token
        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Method for removing the authorization token
        public void RemoveAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}