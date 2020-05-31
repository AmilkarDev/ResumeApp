using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    // This class for when dealking with db context and files saved on the db
    public class CvFile
    {
        public int Id { get; set; }
        public string fileName { get; set; }
        //public int MyProperty { get; set; }
                                           //public virtual Profile Profile { get; set; }

    }
}