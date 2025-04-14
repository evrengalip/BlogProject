using Microsoft.AspNetCore.Mvc;
using BlogProject.Entity.DTOs.Users;
using BlogProject.Web.Services;

namespace BlogProject.Web.Areas.Admin.ViewComponents
{
    public class DashboardHeaderViewComponent : ViewComponent
    {
        private readonly AuthApiService _authService;

        public DashboardHeaderViewComponent(AuthApiService authService)
        {
            _authService = authService;
        }

        public IViewComponentResult Invoke()
        {
            if (_authService.IsAuthenticated())
            {
                var userDto = new UserDto
                {
                    FirstName = HttpContext.Session.GetString("FirstName"),
                    LastName = HttpContext.Session.GetString("LastName"),
                    Role = HttpContext.Session.GetString("Roles")
                };

                return View(userDto);
            }

            return View(new UserDto());
        }
    }
}