using BlogProject.Entity.DTOs.Categories;

namespace BlogProject.Web.Services
{
    public class DashboardApiService
    {
        private readonly ApiClient _apiClient;
        private const string Endpoint = "dashboard";

        public DashboardApiService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<int>> GetYearlyArticleCountsAsync()
        {
            try
            {
                return await _apiClient.GetAsync<List<int>>($"{Endpoint}/yearly-article-counts");
            }
            catch (Exception ex)
            {
                // Hata durumunda boş bir liste döndürelim
                Console.WriteLine($"Yıllık makale sayısı alınırken hata: {ex.Message}");
                return new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // 12 ay için varsayılan değerler
            }
        }

        public async Task<int> GetTotalArticleCountAsync()
        {
            try
            {
                return await _apiClient.GetAsync<int>($"{Endpoint}/total-article-count");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Toplam makale sayısı alınırken hata: {ex.Message}");
                return 0; // Hata durumunda 0 döndür
            }
        }

        public async Task<int> GetTotalCategoryCountAsync()
        {
            try
            {
                return await _apiClient.GetAsync<int>($"{Endpoint}/total-category-count");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Toplam kategori sayısı alınırken hata: {ex.Message}");
                return 0; // Hata durumunda 0 döndür
            }
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                return await _apiClient.GetAsync<List<CategoryDto>>("categories");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kategoriler alınırken hata: {ex.Message}");
                return new List<CategoryDto>();
            }
        }


    }
}
    