// BlogProject.Entity/Entities/Comment.cs
using BlogProject.Core.Entities;
using BlogProject.Entity.Entities;

namespace BlogProject.Entity.Entities
{
    public class Comment : EntityBase
    {
        public Comment()
        {
        }

        public Comment(string text, Guid articleId, Guid userId)
        {
            Text = text;
            ArticleId = articleId;
            UserId = userId;
        }

        public string Text { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
    }
}