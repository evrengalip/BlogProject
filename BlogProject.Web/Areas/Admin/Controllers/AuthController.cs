﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult Register()
        {
            if (_authApiService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if (ModelState.IsValid)
            {
                // Admin area üzerinden yapılan kayıtlar için Admin rolünü atamak için özel bir bayrak gönderiyoruz
                var result = await _authApiService.RegisterAsync(userRegisterDto, isAdminRegistration: true);
                if (result.Succeeded)
                {
                    _toastNotification.AddSuccessToastMessage("Admin hesabınız başarıyla oluşturuldu. Giriş yapabilirsiniz.",
                        new ToastrOptions { Title = "Kayıt Başarılı" });

                    return RedirectToAction("Login", "Auth", new { Area = "Admin" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        switch (error)
                        {
                            case "PasswordRequiresNonAlphanumeric":
                                ModelState.AddModelError("Password", "Şifre en az bir özel karakter içermelidir (örn: !@#$%^&*).");
                                break;
                            case "PasswordRequiresLower":
                                ModelState.AddModelError("Password", "Şifre en az bir küçük harf içermelidir (a-z).");
                                break;
                            case "PasswordRequiresUpper":
                                ModelState.AddModelError("Password", "Şifre en az bir büyük harf içermelidir (A-Z).");
                                break;
                            case "PasswordRequiresDigit":
                                ModelState.AddModelError("Password", "Şifre en az bir rakam içermelidir (0-9).");
                                break;
                            case "PasswordTooShort":
                                ModelState.AddModelError("Password", "Şifre en az 6 karakter uzunluğunda olmalıdır.");
                                break;
                            case "DublicateEmail":
                            case "DuplicateEmail":
                                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                                break;
                            default:
                                ModelState.AddModelError("", error);
                                break;
                        }
                    }
                    return View(userRegisterDto);
                }
            }

            return View(userRegisterDto);
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