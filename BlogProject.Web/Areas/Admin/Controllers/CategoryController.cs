using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Categories;
using BlogProject.Web.Services;
using BlogProject.Web.ResultMessages;

namespace BlogProject.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryApiService _categoryService;
        private readonly IToastNotification _toastNotification;

        public CategoryController(CategoryApiService categoryService, IToastNotification toastNotification)
        {
            _categoryService = categoryService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> DeletedCategory()
        {
            var categories = await _categoryService.GetAllDeletedCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.CreateCategoryAsync(categoryAddDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name), new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "Category", new { Area = "Admin" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Kategori eklenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
                }
            }

            return View(categoryAddDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddWithAjax([FromBody] CategoryAddDto categoryAddDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.CreateCategoryAsync(categoryAddDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name), new ToastrOptions { Title = "İşlem Başarılı" });
                    return Json(new { success = true, message = Messages.Category.Add(categoryAddDto.Name) });
                }
                else
                {
                    return Json(new { success = false, message = "Kategori eklenirken bir hata oluştu." });
                }
            }

            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = errorMessages.FirstOrDefault() });
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            var categoryUpdateDto = new CategoryUpdateDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryUpdateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateDto categoryUpdateDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage(Messages.Category.Update(categoryUpdateDto.Name), new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "Category", new { Area = "Admin" });
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Kategori güncellenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
                }
            }

            return View(categoryUpdateDto);
        }

        public async Task<IActionResult> Delete(Guid categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            var result = await _categoryService.DeleteCategoryAsync(categoryId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(Messages.Category.Delete(category.Name), new ToastrOptions() { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Kategori silinirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }

        public async Task<IActionResult> UndoDelete(Guid categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            var result = await _categoryService.UndoDeleteCategoryAsync(categoryId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage(Messages.Category.UndoDelete(category.Name), new ToastrOptions() { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Kategori geri alınırken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
    }
}