using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogProject.Web.Services;
using System.Security.Claims;

namespace BlogProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ArticleApiService _articleService;
        private readonly DashboardApiService _dashboardService;

        public HomeController(ArticleApiService articleService, DashboardApiService dashboardService)
        {
            _articleService = articleService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> YearlyArticleCounts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articles = await _articleService.GetAllArticlesAsync();

            var userArticles = articles.Where(a => a.User.Id.ToString() == userId && !a.IsDeleted).ToList();

            var monthlyCounts = Enumerable.Range(1, 12)
                .Select(month => userArticles.Count(a => a.CreatedDate.Month == month && a.CreatedDate.Year == DateTime.Now.Year))
                .ToArray();

            return Json(monthlyCounts);
        }


        [HttpGet]
        public async Task<IActionResult> TotalArticleCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articles = await _articleService.GetAllArticlesAsync();
            var count = articles.Count(x => x.User.Id.ToString() == userId && !x.IsDeleted);
            return Json(count);
        }


        [HttpGet]
        public async Task<IActionResult> TotalCategoryCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articles = await _articleService.GetAllArticlesAsync();

            // Kullanıcının yazdığı makalelere göre kaç farklı kategori kullanılmış?
            var categoryIds = articles
                .Where(a => a.User.Id.ToString() == userId && !a.IsDeleted)
                .Select(a => a.Category.Id)
                .Distinct()
                .Count();

            return Json(categoryIds);
        }

    }
}