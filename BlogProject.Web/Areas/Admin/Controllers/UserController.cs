using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Users;
using BlogProject.Web.Services;
using BlogProject.Web.ResultMessages;

namespace BlogProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserApiService _userService;
        private readonly IToastNotification _toastNotification;

        public UserController(UserApiService userService, IToastNotification toastNotification)
        {
            _userService = userService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await _userService.GetAllRolesAsync();
            return View(new UserAddDto { Roles = roles });
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(userAddDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.User.Add(userAddDto.Email), new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Kullanıcı eklenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
                    var roles = await _userService.GetAllRolesAsync();
                    return View(new UserAddDto { Roles = roles });
                }
            }

            var rolesForError = await _userService.GetAllRolesAsync();
            userAddDto.Roles = rolesForError;
            return View(userAddDto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var roles = await _userService.GetAllRolesAsync();

            var userUpdateDto = new UserUpdateDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = Guid.Parse(roles.First(r => r.Name == user.Role).Id.ToString()),
                Roles = roles
            };

            return View(userUpdateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(userUpdateDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.User.Update(userUpdateDto.Email), new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Kullanıcı güncellenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
                    var roles = await _userService.GetAllRolesAsync();
                    userUpdateDto.Roles = roles;
                    return View(userUpdateDto);
                }
            }

            var rolesForError = await _userService.GetAllRolesAsync();
            userUpdateDto.Roles = rolesForError;
            return View(userUpdateDto);
        }

        public async Task<IActionResult> Delete(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var result = await _userService.DeleteUserAsync(userId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(Messages.User.Delete(user.Email), new ToastrOptions { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Kullanıcı silinirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Index", "User", new { Area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await _userService.GetUserProfileAsync();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileDto userProfileDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserProfileAsync(userProfileDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage("Profil güncelleme işlemi tamamlandı", new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Profil güncelleme işlemi tamamlanamadı", new ToastrOptions { Title = "İşlem Başarısız" });
                    return View(userProfileDto);
                }
            }

            return View(userProfileDto);
        }
    }
}