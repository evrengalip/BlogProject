using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Users;
using BlogProject.Web.Services;

namespace BlogProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly AuthApiService _authApiService;
        private readonly IToastNotification _toastNotification;

        public AuthController(AuthApiService authApiService, IToastNotification toastNotification)
        {
            _authApiService = authApiService;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (_authApiService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _authApiService.LoginAsync(userLoginDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage("Başarıyla giriş yaptınız.", new ToastrOptions { Title = "İşlem Başarılı" });

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "E-posta adresiniz veya şifreniz yanlıştır.");
                    return View(userLoginDto);
                }
            }

            return View(userLoginDto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authApiService.LogoutAsync();
            _toastNotification.AddSuccessToastMessage("Başarıyla çıkış yaptınız.", new ToastrOptions { Title = "İşlem Başarılı" });
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}