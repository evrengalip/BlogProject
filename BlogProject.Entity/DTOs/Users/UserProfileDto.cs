﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogProject.Entity.Entities;

namespace BlogProject.Entity.DTOs.Users
{
    public class UserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ImagePath { get; set; } // sadece gösterim için

    }
}