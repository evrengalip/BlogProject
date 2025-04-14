using Microsoft.AspNetCore.Mvc.Filters;
using BlogProject.Web.Services;

namespace BlogProject.Web.Filters.ArticleVisitors
{
    public class ArticleVisitorFilter : IAsyncActionFilter
    {
        // Bu filtre API kullanıldığında farklı olacaktır, çünkü ziyaretçi takibi API tarafında yapılacaktır
        // Bu örnekte basitçe gerektiğinde API'ye ziyaretçi bilgisi gönderebiliriz

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleVisitorFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // API'ye ziyaretçi bilgisi gönderme işlemi buraya eklenebilir
            // Şu an sadece filteri atlamasını söylüyoruz
            await next();
        }
    }
}