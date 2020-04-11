using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class FullEmail
    {
        public List<string> Attachments { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string EmailContent { get; set; }
    }
}