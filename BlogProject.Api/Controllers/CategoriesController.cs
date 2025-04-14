using BlogProject.Entity.DTOs.Categories;
using BlogProject.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return Ok(categories);
        }

        [HttpGet("top24")]
        public async Task<IActionResult> GetTop24()
        {
            var categories = await _categoryService.GetAllCategoriesNonDeletedTake24();
            return Ok(categories);
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> GetAllDeleted()
        {
            var categories = await _categoryService.GetAllCategoriesDeleted();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetCategoryByGuid(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryAddDto categoryAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.CreateCategoryAsync(categoryAddDto);
            return StatusCode(201, new { message = "Category created successfully" });
        }

        [HttpPut]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var name = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);
            return Ok(new { message = $"Category '{name}' updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var name = await _categoryService.SafeDeleteCategoryAsync(id);
            return Ok(new { message = $"Category '{name}' deleted successfully" });
        }

        [HttpPut("undo-delete/{id}")]
        [Authorize(Roles = "Superadmin,Admin")]
        public async Task<IActionResult> UndoDelete(Guid id)
        {
            var name = await _categoryService.UndoDeleteCategoryAsync(id);
            return Ok(new { message = $"Category '{name}' restored successfully" });
        }
    }
}