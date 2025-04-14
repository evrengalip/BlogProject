using BlogProject.Entity.DTOs.Users;
using BlogProject.Entity.Entities;

namespace BlogProject.Web.Services
{
    public class UserApiService
    {
        private readonly ApiClient _apiClient;
        private const string Endpoint = "users";

        public UserApiService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _apiClient.GetAsync<List<UserDto>>(Endpoint);
        }

        public async Task<List<AppRole>> GetAllRolesAsync()
        {
            return await _apiClient.GetAsync<List<AppRole>>($"{Endpoint}/roles");
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            return await _apiClient.GetAsync<UserDto>($"{Endpoint}/{id}");
        }

        public async Task<bool> CreateUserAsync(UserAddDto userAddDto)
        {
            try
            {
                await _apiClient.PostAsync<object, UserAddDto>(Endpoint, userAddDto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            try
            {
                await _apiClient.PutAsync<object, UserUpdateDto>($"{Endpoint}/{userUpdateDto.Id}", userUpdateDto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
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

        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            return await _apiClient.GetAsync<UserProfileDto>($"{Endpoint}/profile");
        }

        public async Task<bool> UpdateUserProfileAsync(UserProfileDto userProfileDto)
        {
            var formData = new MultipartFormDataContent();

            // Add form fields
            formData.Add(new StringContent(userProfileDto.FirstName), "FirstName");
            formData.Add(new StringContent(userProfileDto.LastName), "LastName");
            formData.Add(new StringContent(userProfileDto.Email), "Email");
            formData.Add(new StringContent(userProfileDto.PhoneNumber), "PhoneNumber");

            if (!string.IsNullOrEmpty(userProfileDto.CurrentPassword))
                formData.Add(new StringContent(userProfileDto.CurrentPassword), "CurrentPassword");

            if (!string.IsNullOrEmpty(userProfileDto.NewPassword))
                formData.Add(new StringContent(userProfileDto.NewPassword), "NewPassword");

            // Add photo if present
            if (userProfileDto.Photo != null)
            {
                using var ms = new MemoryStream();
                await userProfileDto.Photo.CopyToAsync(ms);
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(userProfileDto.Photo.ContentType);
                formData.Add(fileContent, "Photo", userProfileDto.Photo.FileName);
            }

            try
            {
                await _apiClient.PutFormAsync<object>($"{Endpoint}/profile", formData);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}