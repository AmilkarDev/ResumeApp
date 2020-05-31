using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResumeApp.Helpers
{
    public class SearchModel
    {
        public SearchModel()
        {
            LanguageIds = new List<int>();
            CountryIds =  new List<int>();
            SkillIds = new List<int>();
            ToolIds = new List<int>();
            TitleIds = new List<int>();
            EntityIds = new List<int>();
            profileIds = new List<int>();
            KeyPhrasesIds = new List<int>();
        }
        public List<int> profileIds { get; set; }
        public List<int> LanguageIds { get; set; }
        public List<int> CountryIds { get; set; }
        public List<int> SkillIds { get; set; }
        public List<int> ToolIds { get; set; }
        public List<int> TitleIds { get; set; }
        public List<int> KeyPhrasesIds { get; set; }
        public List<int> EntityIds { get; set; }
    }
}