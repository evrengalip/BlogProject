using BlogProject.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using BlogProject.Core.Entities;

namespace BlogProject.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid? ImageId { get; set; }
        public Image? Image { get; set; }

        public ICollection<Article> Articles { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}