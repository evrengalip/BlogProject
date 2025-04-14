using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Articles;
using BlogProject.Web.Consts;
using BlogProject.Web.Services;
using BlogProject.Web.ResultMessages;

namespace BlogProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}, {RoleConsts.User}")]
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return View(articles);
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> DeletedArticle()
        {
            var articles = await _articleService.GetAllDeletedArticlesAsync();
            return View(articles);
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(new ArticleAddDto { Categories = categories });
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> Add(ArticleAddDto articleAddDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _articleService.CreateArticleAsync(articleAddDto);
                    if (result)
                    {
                        _toastNotification.AddSuccessToastMessage(Messages.Article.Add(articleAddDto.Title), new ToastrOptions { Title = "İşlem Başarılı" });
                        return RedirectToAction("Index", "Article", new { Area = "Admin" });
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Makale eklenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
                        var categories = await _categoryService.GetAllCategoriesAsync();
                        articleAddDto.Categories = categories;
                        return View(articleAddDto);
                    }
                }
                catch (Exception ex)
                {
                    // Detaylı hata bilgisi gösteriyoruz
                    _toastNotification.AddErrorToastMessage($"Hata: {ex.Message}", new ToastrOptions { Title = "İşlem Başarısız" });
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    articleAddDto.Categories = categories;
                    return View(articleAddDto);
                }
            }

            var categoriesForError = await _categoryService.GetAllCategoriesAsync();
            articleAddDto.Categories = categoriesForError;
            return View(articleAddDto);
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> Update(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            var categories = await _categoryService.GetAllCategoriesAsync();

            var articleUpdateDto = new ArticleUpdateDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CategoryId = article.Category.Id,
                Image = article.Image,
                Categories = categories
            };

            return View(articleUpdateDto);
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> Update(ArticleUpdateDto articleUpdateDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _articleService.UpdateArticleAsync(articleUpdateDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.Article.Update(articleUpdateDto.Title), new ToastrOptions() { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "Article", new { Area = "Admin" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Makale güncellenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
                    var categories = await _categoryService.GetAllCategoriesAsync();
                    articleUpdateDto.Categories = categories;
                    return View(articleUpdateDto);
                }
            }

            var categoriesForError = await _categoryService.GetAllCategoriesAsync();
            articleUpdateDto.Categories = categoriesForError;
            return View(articleUpdateDto);
        }

        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> Delete(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            var result = await _articleService.DeleteArticleAsync(articleId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(Messages.Article.Delete(article.Title), new ToastrOptions() { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Makale silinirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }

        [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
        public async Task<IActionResult> UndoDelete(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            var result = await _articleService.UndoDeleteArticleAsync(articleId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(Messages.Article.UndoDelete(article.Title), new ToastrOptions() { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Makale geri alınırken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
    }
}