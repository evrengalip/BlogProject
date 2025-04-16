// BlogProject.Web/Services/VisitorApiService.cs
namespace BlogProject.Web.Services
{
    public class VisitorApiService
    {
        private readonly ApiClient _apiClient;
        private const string Endpoint = "articles";

        public VisitorApiService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> AddArticleVisitorAsync(Guid articleId, string ipAddress, string userAgent)
        {
            try
            {
                var visitorDto = new
                {
                    ArticleId = articleId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                await _apiClient.PostAsync<object, object>($"{Endpoint}/add-visitor", visitorDto);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}