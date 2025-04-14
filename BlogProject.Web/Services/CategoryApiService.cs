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

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            return await _apiClient.GetAsync<CategoryDto>($"{Endpoint}/{id}");
        }

        public async Task<bool> CreateCategoryAsync(CategoryAddDto categoryAddDto)
        {
            try
            {
                await _apiClient.PostAsync<object, CategoryAddDto>(Endpoint, categoryAddDto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            try
            {
                await _apiClient.PutAsync<object, CategoryUpdateDto>($"{Endpoint}/{categoryUpdateDto.Id}", categoryUpdateDto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
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

        public async Task<bool> UndoDeleteCategoryAsync(Guid id)
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