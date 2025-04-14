using BlogProject.Entity.DTOs.Comments;
using BlogProject.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("article/{articleId}")]
        public async Task<IActionResult> GetByArticleId(Guid articleId)
        {
            var comments = await _commentService.GetAllCommentsByArticleIdAsync(articleId);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentAddDto commentAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _commentService.CreateCommentAsync(commentAddDto);
            return StatusCode(201, new { message = "Comment added successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var text = await _commentService.DeleteCommentAsync(id);
            return Ok(new { message = "Comment deleted successfully" });
        }
    }
}