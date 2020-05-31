using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class Entity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public double Score { get; set; }
        public virtual Profile Profile { get; set; }
    }
}