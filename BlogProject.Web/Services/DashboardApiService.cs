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
            return await _apiClient.GetAsync<List<int>>($"{Endpoint}/yearly-article-counts");
        }

        public async Task<int> GetTotalArticleCountAsync()
        {
            return await _apiClient.GetAsync<int>($"{Endpoint}/total-article-count");
        }

        public async Task<int> GetTotalCategoryCountAsync()
        {
            return await _apiClient.GetAsync<int>($"{Endpoint}/total-category-count");
        }
    }
}