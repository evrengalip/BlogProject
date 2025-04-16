// BlogProject.Web/ViewComponents/RecentPostsViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using BlogProject.Web.Services;

namespace BlogProject.Web.ViewComponents
{
    public class RecentPostsViewComponent : ViewComponent
    {
        private readonly ArticleApiService _articleService;

        public RecentPostsViewComponent(ArticleApiService articleService)
        {
            _articleService = articleService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 3)
        {
            var articles = await _articleService.GetAllArticlesAsync();
            var recentArticles = articles
                .OrderByDescending(a => a.CreatedDate)
                .Take(count)
                .ToList();

            return View(recentArticles);
        }
    }
}