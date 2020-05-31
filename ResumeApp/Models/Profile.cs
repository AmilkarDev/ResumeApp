using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ResumeApp.Models
{
    public class Profile
    {
        public Profile()
        {
            this.Languages = new HashSet<Language>();
            this.Skills = new HashSet<Skill>();
            this.Titles = new HashSet<Title>();
            this.Countries = new HashSet<Country>();
            this.Tools = new HashSet<Tool>();
            this.Entities = new HashSet<Entity>();
            this.KeyPhrases = new HashSet<KeyPhrase>();
        }
        public int Id { get; set; }
        [ForeignKey("CvFile")]
        public int CvFileId { get; set; }
        public string FullName { get; set; }
        public string  PhoneNumber { get; set; }
        public string Email { get; set; }
        public ICollection<Country> Countries { get; set; }
        public ICollection<Language> Languages { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public ICollection<Title> Titles { get; set; }
        public ICollection<Tool> Tools { get; set; }
        public ICollection<Entity> Entities { get; set; }
        public ICollection<KeyPhrase> KeyPhrases { get; set; }
        public string Nationality { get; set; }
        public bool Validated { get; set; }
        public virtual CvFile CvFile { get; set; }

    }
}