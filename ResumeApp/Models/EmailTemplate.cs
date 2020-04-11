using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public enum names {
        [Display(Name ="cv To Colleague")]
        cvToColleague ,
        [Display(Name = "cv To Client")]
        cvToClient
    };
    public class EmailTemplate
    {
        public names Name { get; set; }
    }
}