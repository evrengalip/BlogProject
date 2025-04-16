using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.Entity.DTOs.Articles
{
    public class ArticleVisitorDto
    {
        public Guid ArticleId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
