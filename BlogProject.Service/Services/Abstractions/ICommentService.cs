// BlogProject.Service/Services/Abstractions/ICommentService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogProject.Entity.DTOs.Comments;
using BlogProject.Entity.Entities;

namespace BlogProject.Service.Services.Abstractions
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllCommentsByArticleIdAsync(Guid articleId);
        Task CreateCommentAsync(CommentAddDto commentAddDto);
        Task<string> DeleteCommentAsync(Guid commentId);
    }
}