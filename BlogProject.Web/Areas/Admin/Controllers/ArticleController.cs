using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Articles;
using BlogProject.Web.Consts;
using BlogProject.Web.ResultMessages;
using BlogProject.Web.Services;
using System.Security.Claims;

namespace BlogProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public class ArticleController : Controller
    {
        private readonly ArticleApiService _articleService;
        private readonly CategoryApiService _categoryService;
        private readonly IToastNotification _toastNotification;

        public ArticleController(
            ArticleApiService articleService,
            CategoryApiService categoryService,
            IToastNotification toastNotification)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("Superadmin"))
            {
                // Sadece kendi yazdığı makaleler
                var allArticles = await _articleService.GetAllArticlesAsync();
                var userArticles = allArticles.Where(a => a.User.Id.ToString() == userId).ToList();
                return View(userArticles);
            }

            // Superadmin ise hepsini görsün
            var articles = await _articleService.GetAllArticlesAsync();
            return View(articles);
        }


        [HttpGet]
        public async Task<IActionResult> DeletedArticle()
        {
            var articles = await _articleService.GetAllDeletedArticlesAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSuperAdmin = User.IsInRole("Superadmin");

            if (!isSuperAdmin)
            {
                articles = articles.Where(a => a.User.Id.ToString() == userId).ToList();
            }

            return View(articles);
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(new ArticleAddDto { Categories = categories });
        }

        [HttpPost]
        public async Task<IActionResult> Add(ArticleAddDto articleAddDto)
        {
            var result = await _articleService.CreateArticleAsync(articleAddDto);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(
                    Messages.Article.Add(articleAddDto.Title),
                    new ToastrOptions { Title = "İşlem Başarılı" });

                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }

            _toastNotification.AddErrorToastMessage("Makale eklenemedi.", new ToastrOptions { Title = "Hata" });

            var categories = await _categoryService.GetAllCategoriesAsync();
            articleAddDto.Categories = categories;
            return View(articleAddDto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var dto = new ArticleUpdateDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CategoryId = article.Category.Id,
                Image = article.Image,
                Categories = categories
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ArticleUpdateDto articleUpdateDto)
        {
            var result = await _articleService.UpdateArticleAsync(articleUpdateDto);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(
                    Messages.Article.Update(articleUpdateDto.Title),
                    new ToastrOptions { Title = "İşlem Başarılı" });

                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }

            _toastNotification.AddErrorToastMessage("Makale güncellenemedi.", new ToastrOptions { Title = "Hata" });

            articleUpdateDto.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(articleUpdateDto);
        }

        public async Task<IActionResult> Delete(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            var result = await _articleService.DeleteArticleAsync(articleId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(
                    Messages.Article.Delete(article.Title),
                    new ToastrOptions { Title = "Silme Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Silme başarısız.", new ToastrOptions { Title = "Hata" });
            }

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }

        public async Task<IActionResult> UndoDelete(Guid articleId)
        {
            try
            {
                // Önce makaleyi geri al
                var result = await _articleService.UndoDeleteArticleAsync(articleId);

                if (result)
                {
                    // Başarılı olduysa, şimdi makale bilgilerini getir
                    var article = await _articleService.GetArticleByIdAsync(articleId);
                    _toastNotification.AddSuccessToastMessage(
                        Messages.Article.UndoDelete(article.Title),
                        new ToastrOptions { Title = "Geri Alındı" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Geri alma başarısız.", new ToastrOptions { Title = "Hata" });
                }
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage($"İşlem sırasında bir hata oluştu: {ex.Message}",
                    new ToastrOptions { Title = "Hata" });
            }

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
    }
}
