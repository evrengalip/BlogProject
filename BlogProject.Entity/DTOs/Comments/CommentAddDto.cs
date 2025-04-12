// BlogProject.Entity/DTOs/Comments/CommentAddDto.cs
using System;

namespace BlogProject.Entity.DTOs.Comments
{
    public class CommentAddDto
    {
        public string Text { get; set; }
        public Guid ArticleId { get; set; }
    }
}