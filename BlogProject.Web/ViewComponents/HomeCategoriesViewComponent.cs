using Microsoft.AspNetCore.Mvc;
using BlogProject.Web.Services;

namespace BlogProject.Web.ViewComponents
{
    public class HomeCategoriesViewComponent : ViewComponent
    {
        private readonly CategoryApiService _categoryService;

        public HomeCategoriesViewComponent(CategoryApiService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetTop24CategoriesAsync();
            return View(categories);
        }
    }
}