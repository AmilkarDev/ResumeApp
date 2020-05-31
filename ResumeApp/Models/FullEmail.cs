using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class FullEmail
    {
        public FullEmail()
        {
            Attachments = new List<string>();
            Recipients = new List<string>();
            ccrecipients = new List<string>();
        }
        public List<string> Attachments { get; set; }
        public List<string> Recipients { get; set; }
        public List<string> ccrecipients { get; set; }
        public string Subject { get; set; }
        public string EmailContent { get; set; }
    }
}