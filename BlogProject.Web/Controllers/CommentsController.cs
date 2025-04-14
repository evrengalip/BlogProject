using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Comments;
using BlogProject.Web.Services;

namespace BlogProject.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly CommentApiService _commentService;
        private readonly IToastNotification _toastNotification;

        public CommentsController(CommentApiService commentService, IToastNotification toastNotification)
        {
            _commentService = commentService;
            _toastNotification = toastNotification;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(CommentAddDto commentAddDto)
        {
            var result = await _commentService.CreateCommentAsync(commentAddDto);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage("Yorum başarıyla eklenmiştir.", new ToastrOptions { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Yorum eklenirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Detail", "Home", new { id = commentAddDto.ArticleId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(Guid commentId, Guid articleId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId);

            if (result)
            {
                _toastNotification.AddSuccessToastMessage("Yorum başarıyla silinmiştir.", new ToastrOptions { Title = "İşlem Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Yorum silinirken bir hata oluştu.", new ToastrOptions { Title = "İşlem Başarısız" });
            }

            return RedirectToAction("Detail", "Home", new { id = articleId });
        }
    }
}