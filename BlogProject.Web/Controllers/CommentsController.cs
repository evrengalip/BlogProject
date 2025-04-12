// BlogProject.Web/Controllers/CommentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using BlogProject.Entity.DTOs.Comments;
using BlogProject.Service.Services.Abstractions;
using BlogProject.Web.ResultMessages;

namespace BlogProject.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService commentService;
        private readonly IToastNotification toast;

        public CommentsController(ICommentService commentService, IToastNotification toast)
        {
            this.commentService = commentService;
            this.toast = toast;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(CommentAddDto commentAddDto)
        {
            await commentService.CreateCommentAsync(commentAddDto);
            toast.AddSuccessToastMessage("Yorum başarıyla eklenmiştir.", new ToastrOptions { Title = "İşlem Başarılı" });

            return RedirectToAction("Detail", "Home", new { id = commentAddDto.ArticleId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(Guid commentId, Guid articleId)
        {
            await commentService.DeleteCommentAsync(commentId);
            toast.AddSuccessToastMessage("Yorum başarıyla silinmiştir.", new ToastrOptions { Title = "İşlem Başarılı" });

            return RedirectToAction("Detail", "Home", new { id = articleId });
        }
    }
}