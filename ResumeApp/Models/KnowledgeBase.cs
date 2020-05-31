using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class KnowledgeBase
    {
        public KnowledgeBase()
        {
            this.Languages = new HashSet<Language>();
            this.Skills = new HashSet<Skill>();
            this.Titles = new HashSet<Title>();
            this.Countries = new HashSet<Country>();
            this.Tools = new HashSet<Tool>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Country> Countries { get; set; }
        public ICollection<Skill> Skills { get; set; }
        //public ICollection<Field> Fields { get; set; }
        public ICollection<Language> Languages { get; set; }
        public ICollection<Title> Titles { get; set; }
        public ICollection<Tool> Tools { get; set; }

    }
}