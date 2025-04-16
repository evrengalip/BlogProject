// BlogProject.Web/Filters/ArticleVisitors/ArticleVisitorFilter.cs
using Microsoft.AspNetCore.Mvc.Filters;
using BlogProject.Web.Services;
using BlogProject.Entity.DTOs.Articles;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Web.Filters.ArticleVisitors
{
    public class ArticleVisitorFilter : IAsyncActionFilter
    {
        private readonly VisitorApiService _visitorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleVisitorFilter(VisitorApiService visitorService, IHttpContextAccessor httpContextAccessor)
        {
            _visitorService = visitorService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Önce işlemi tamamla
            var resultContext = await next();

            // Eğer action'ın adı Detail ise ve başarılıysa ziyaretçi kaydı yapılır
            if (context.ActionDescriptor.DisplayName.Contains("Detail") && resultContext.Result is ViewResult viewResult)
            {
                if (viewResult.Model is ArticleDto articleDto)
                {
                    // Ziyaretçi bilgilerini al
                    string ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    string userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];

                    // Makale ziyaretini kaydet
                    await _visitorService.AddArticleVisitorAsync(articleDto.Id, ipAddress, userAgent);
                }
            }
        }
    }
}