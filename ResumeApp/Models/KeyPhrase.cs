using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class KeyPhrase
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual Profile Profile { get; set; }
    }
}