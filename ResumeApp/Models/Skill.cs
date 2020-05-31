﻿using System.Collections.Generic;

namespace ResumeApp.Models
{
    public class Skill
    {
        public Skill()
        {
            this.KnowledgeBases = new HashSet<KnowledgeBase>();
            this.Profiles = new HashSet<Profile>();
        }
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<KnowledgeBase> KnowledgeBases { get; set; }
        public ICollection<Profile> Profiles { get; set; }
    }
}