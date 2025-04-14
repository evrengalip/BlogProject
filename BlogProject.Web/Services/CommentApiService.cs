using BlogProject.Entity.DTOs.Comments;

namespace BlogProject.Web.Services
{
    public class CommentApiService
    {
        private readonly ApiClient _apiClient;
        private const string Endpoint = "comments";

        public CommentApiService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId)
        {
            return await _apiClient.GetAsync<List<CommentDto>>($"{Endpoint}/article/{articleId}");
        }

        public async Task<object> CreateCommentAsync(CommentAddDto commentAddDto)
        {
            return await _apiClient.PostAsync<object, CommentAddDto>(Endpoint, commentAddDto);
        }

        public async Task<object> DeleteCommentAsync(Guid id)
        {
            return await _apiClient.DeleteAsync<object>($"{Endpoint}/{id}");
        }
    }
}