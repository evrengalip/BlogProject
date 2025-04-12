// BlogProject.Entity/DTOs/Comments/CommentDto.cs
using System;
using BlogProject.Entity.Entities;

namespace BlogProject.Entity.DTOs.Comments
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserImageUrl { get; set; }
    }
}