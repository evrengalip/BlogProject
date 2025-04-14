using BlogProject.Entity.DTOs.Articles;
using Microsoft.AspNetCore.Http;

namespace BlogProject.Web.Services
{
    public class ArticleApiService
    {
        private readonly ApiClient _apiClient;
        private const string Endpoint = "articles";

        public ArticleApiService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<ArticleDto>> GetAllArticlesAsync()
        {
            return await _apiClient.GetAsync<List<ArticleDto>>(Endpoint);
        }

        public async Task<List<ArticleDto>> GetAllDeletedArticlesAsync()
        {
            return await _apiClient.GetAsync<List<ArticleDto>>($"{Endpoint}/deleted");
        }

        public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
        {
            return await _apiClient.GetAsync<ArticleDto>($"{Endpoint}/{id}");
        }

        public async Task<ArticleListDto> GetArticlesByPagingAsync(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            var query = $"{Endpoint}/paged?currentPage={currentPage}&pageSize={pageSize}&isAscending={isAscending}";

            if (categoryId.HasValue)
                query += $"&categoryId={categoryId}";

            return await _apiClient.GetAsync<ArticleListDto>(query);
        }

        public async Task<ArticleListDto> SearchArticlesAsync(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            var query = $"{Endpoint}/search?keyword={Uri.EscapeDataString(keyword)}&currentPage={currentPage}&pageSize={pageSize}&isAscending={isAscending}";

            return await _apiClient.GetAsync<ArticleListDto>(query);
        }

        public async Task<bool> CreateArticleAsync(ArticleAddDto articleAddDto)
        {
            try
            {
                var formData = new MultipartFormDataContent();

                // Form alanlarını ekleyin
                formData.Add(new StringContent(articleAddDto.Title), "Title");
                formData.Add(new StringContent(articleAddDto.Content), "Content");
                formData.Add(new StringContent(articleAddDto.CategoryId.ToString()), "CategoryId");

                // Eğer fotoğraf varsa ekleyin
                if (articleAddDto.Photo != null)
                {
                    using var ms = new MemoryStream();
                    await articleAddDto.Photo.CopyToAsync(ms);
                    ms.Position = 0;
                    var fileContent = new ByteArrayContent(ms.ToArray());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(articleAddDto.Photo.ContentType);
                    formData.Add(fileContent, "Photo", articleAddDto.Photo.FileName);
                }

                // API çağrısını yapın ve sonucu kontrol edin
                await _apiClient.PostFormAsync<object>(Endpoint, formData);
                return true;
            }
            catch (Exception ex)
            {
                // Hata loglama
                Console.WriteLine($"Makale eklenirken hata oluştu: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");

                return false;
            }
        }

        public async Task<bool> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto)
        {
            var formData = new MultipartFormDataContent();

            // Add form fields
            formData.Add(new StringContent(articleUpdateDto.Id.ToString()), "Id");
            formData.Add(new StringContent(articleUpdateDto.Title), "Title");
            formData.Add(new StringContent(articleUpdateDto.Content), "Content");
            formData.Add(new StringContent(articleUpdateDto.CategoryId.ToString()), "CategoryId");

            // Add image if present
            if (articleUpdateDto.Photo != null)
            {
                using var ms = new MemoryStream();
                await articleUpdateDto.Photo.CopyToAsync(ms);
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(articleUpdateDto.Photo.ContentType);
                formData.Add(fileContent, "Photo", articleUpdateDto.Photo.FileName);
            }

            try
            {
                await _apiClient.PutFormAsync<object>($"{Endpoint}/{articleUpdateDto.Id}", formData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteArticleAsync(Guid id)
        {
            try
            {
                await _apiClient.DeleteAsync<object>($"{Endpoint}/{id}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UndoDeleteArticleAsync(Guid id)
        {
            try
            {
                await _apiClient.PutAsync<object, object>($"{Endpoint}/undo-delete/{id}", null);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}