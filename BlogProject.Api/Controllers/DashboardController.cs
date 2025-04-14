using BlogProject.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Superadmin,Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashbordService _dashboardService;

        public DashboardController(IDashbordService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("yearly-article-counts")]
        public async Task<IActionResult> GetYearlyArticleCounts()
        {
            var counts = await _dashboardService.GetYearlyArticleCounts();
            return Ok(counts);
        }

        [HttpGet("total-article-count")]
        public async Task<IActionResult> GetTotalArticleCount()
        {
            var count = await _dashboardService.GetTotalArticleCount();
            return Ok(count);
        }

        [HttpGet("total-category-count")]
        public async Task<IActionResult> GetTotalCategoryCount()
        {
            var count = await _dashboardService.GetTotalCategoryCount();
            return Ok(count);
        }
    }
}