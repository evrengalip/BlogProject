using BlogProject.Entity.DTOs.Categories;
using BlogProject.Entity.Entities;

namespace BlogProject.Web.Services
{
    public class CategoryApiService
    {
        private readonly ApiClient _apiClient;
        private const string Endpoint = "categories";

        public CategoryApiService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _apiClient.GetAsync<List<CategoryDto>>(Endpoint);
        }

        public async Task<List<CategoryDto>> GetTop24CategoriesAsync()
        {
            return await _apiClient.GetAsync<List<CategoryDto>>($"{Endpoint}/top24");
        }

        public async Task<List<CategoryDto>> GetAllDeletedCategoriesAsync()
        {
            return await _apiClient.GetAsync<List<CategoryDto>>($"{Endpoint}/deleted");
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            return await _apiClient.GetAsync<Category>($"{Endpoint}/{id}");
        }

        public async Task<object> CreateCategoryAsync(CategoryAddDto categoryAddDto)
        {
            return await _apiClient.PostAsync<object, CategoryAddDto>(Endpoint, categoryAddDto);
        }

        public async Task<object> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            return await _apiClient.PutAsync<object, CategoryUpdateDto>(Endpoint, categoryUpdateDto);
        }

        public async Task<object> DeleteCategoryAsync(Guid id)
        {
            return await _apiClient.DeleteAsync<object>($"{Endpoint}/{id}");
        }

        public async Task<object> UndoDeleteCategoryAsync(Guid id)
        {
            return await _apiClient.PutAsync<object, object>($"{Endpoint}/undo-delete/{id}", null);
        }
    }
}