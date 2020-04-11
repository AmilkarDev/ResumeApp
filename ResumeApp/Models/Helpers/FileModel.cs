using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ResumeApp.Models.Helpers
{
    public class FileModel
    {
        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "sélectionner les CVs ")]
        public HttpPostedFileBase[] files { get; set; }
    }
}