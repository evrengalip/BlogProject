using BlogProject.Entity.DTOs.Articles;
using BlogProject.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            return Ok(articles);
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> GetAllDeleted()
        {
            var articles = await _articleService.GetAllArticlesWithCategoryDeletedAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var article = await _articleService.GetArticleWithCategoryNonDeletedAsync(id);
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? categoryId,
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 3,
            [FromQuery] bool isAscending = false)
        {
            var articles = await _articleService.GetAllByPagingAsync(
                categoryId, currentPage, pageSize, isAscending);

            return Ok(articles);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string keyword,
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 3,
            [FromQuery] bool isAscending = false)
        {
            var articles = await _articleService.SearchAsync(
                keyword, currentPage, pageSize, isAscending);

            return Ok(articles);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> Create([FromForm] ArticleAddDto articleAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _articleService.CreateArticleAsync(articleAddDto);
            return StatusCode(201, new { message = "Article created successfully" });
        }

        [HttpPut]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> Update([FromForm] ArticleUpdateDto articleUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var title = await _articleService.UpdateArticleAsync(articleUpdateDto);
            return Ok(new { message = $"Article '{title}' updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var title = await _articleService.SafeDeleteArticleAsync(id);
            return Ok(new { message = $"Article '{title}' deleted successfully" });
        }

        [HttpPut("undo-delete/{id}")]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> UndoDelete(Guid id)
        {
            var title = await _articleService.UndoDeleteArticleAsync(id);
            return Ok(new { message = $"Article '{title}' restored successfully" });
        }
    }
}