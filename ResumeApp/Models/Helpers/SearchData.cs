using Microsoft.Azure.Search.Models;
using ResumeApp.Models.Helpers;

namespace ResumeApp.Models.Helpers
{
    public class SearchData
    {
        public string searchText { get; set; }

        // The list of results.
        public DocumentSearchResult<ResumeFile> resultList;
    }
}