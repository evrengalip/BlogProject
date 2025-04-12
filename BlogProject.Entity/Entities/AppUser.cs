using BlogProject.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using BlogProject.Core.Entities;

namespace BlogProject.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid ImageId { get; set; } = Guid.Parse("4084c97a-2aa1-4675-b519-69f6fe410633");
        public Image Image { get; set; }

        public ICollection<Article> Articles { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}