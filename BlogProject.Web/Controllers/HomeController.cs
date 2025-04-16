using BlogProject.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlogProject.Web.Services;

namespace BlogProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ArticleApiService _articleService;
        private readonly CategoryApiService _categoryService;
        private readonly CommentApiService _commentService;

        public HomeController(
            ILogger<HomeController> logger,
            ArticleApiService articleService,
            CategoryApiService categoryService,
            CommentApiService commentService)
        {
            _logger = logger;
            _articleService = articleService;
            _categoryService = categoryService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            try
            {
                var articles = await _articleService.GetArticlesByPagingAsync(categoryId, currentPage, pageSize, isAscending);
                return View(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana sayfa yüklenirken hata oluþtu");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            try
            {
                var articles = await _articleService.SearchArticlesAsync(keyword, currentPage, pageSize, isAscending);
                return View(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama yapýlýrken hata oluþtu");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);

                
                if (article == null)
                    return NotFound();

                return View(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Makale detayý görüntülenirken hata oluþtu");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}