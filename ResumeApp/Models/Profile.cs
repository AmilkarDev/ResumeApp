using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string  PhoneNumber { get; set; }
        public string Email { get; set; }

    }
}