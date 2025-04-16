using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogProject.Entity.DTOs.Visitors
{
    public class VisitorAddDto
    {
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
