using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using ResumeApp.Models;
using ResumeApp.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ResumeApp.Controllers
{
    public class AdvSearchController : Controller
    {
        SearchData modelOne;
        public AdvSearchController() { }
        ApplicationDbContext ctx = new ApplicationDbContext();
        // GET: AdvSearch
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> FuzzySearch(string searchTerm)
        {
            searchTerm = searchTerm + "~";
            List<string> highlights = new List<string> { "content" };
            List<string> searchFields = new List<string> { "content" };
            var parameters = new SearchParameters
            {
                QueryType = QueryType.Full,
                HighlightFields = highlights,
                HighlightPreTag = "<em style='color: blue'><b>",
                HighlightPostTag = "</b></em>",
                SearchMode = SearchMode.Any,
                SearchFields = searchFields,
                Select = new[] { "metadata_storage_name" }
            };
            SearchData model = new SearchData { searchText = searchTerm };
            try
            {
                // Ensure the search string is valid.
                if (model.searchText == null)
                {
                    model.searchText = "";
                }
                // Make the Azure Cognitive Search call.
                await RunQueryAsync(model, parameters);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });
            }
            ViewBag.Count = model.resultList.Results.Count();
            return PartialView(model);
        }
        public async Task<ActionResult> WildCardSearch(string searchTerm)
        {
            searchTerm = searchTerm + "*";
            List<string> highlights = new List<string> { "content" };
            List<string> searchFields = new List<string> { "content" };
            var parameters = new SearchParameters
            {
                HighlightFields = highlights,
                HighlightPreTag = "<em style='color: blue'><b>",
                HighlightPostTag = "</b></em>",
                SearchMode = SearchMode.Any,
                SearchFields = searchFields,
                Select = new[] { "metadata_storage_name" }
            };
            SearchData model = new SearchData { searchText = searchTerm };
            try
            {
                // Ensure the search string is valid.
                if (model.searchText == null)
                {
                    model.searchText = "";
                }
                // Make the Azure Cognitive Search call.
                await RunQueryAsync(model, parameters);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });
            }
            return PartialView(model);
        }
        public async Task<ActionResult> AnySearch(string searchTerm)
        {
            List<string> highlights = new List<string> { "content" };
            List<string> searchFields = new List<string> { "content" };
            var parameters = new SearchParameters
            {
                HighlightFields = highlights,
                HighlightPreTag = "<em style='color: blue'><b>",
                HighlightPostTag = "</b></em>",
                SearchMode = SearchMode.Any,
                SearchFields = searchFields,
                Select = new[] { "metadata_storage_name" }
            };
            SearchData model = new SearchData { searchText = searchTerm };
            try
            {
                // Ensure the search string is valid.
                if (model.searchText == null)
                {
                    model.searchText = "";
                }
                // Make the Azure Cognitive Search call.
                await RunQueryAsync(model, parameters);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });
            }
            return PartialView(model);
        }
        public async Task<ActionResult> AllSearch(string searchTerm)
        {
            List<string> highlights = new List<string> { "content" };
            List<string> searchFields = new List<string> { "content" };
            var parameters = new SearchParameters
            {
                HighlightFields = highlights,
                HighlightPreTag = "<em style='color: blue'>",
                HighlightPostTag = "</em>",
                SearchMode = SearchMode.All,
                SearchFields = searchFields,
                Select = new[] { "metadata_storage_name" }
            };
           SearchData model = new SearchData { searchText = searchTerm };
            try
            {
                // Ensure the search string is valid.
                if (model.searchText == null)
                {
                    model.searchText = "";
                }
                // Make the Azure Cognitive Search call.
                await RunQueryAsync(model,parameters);
            }
            catch
            {
                return View("Error", new ErrorViewModel { RequestId = "1" });
            }
            return PartialView(model);
        }
        private static SearchServiceClient _serviceClient;
        private static ISearchIndexClient _indexClient;


        public ActionResult Error()
        {
            return View();
        }
        private void InitSearch()
        {
            // Create a configuration using the appsettings file.
            //_builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //_configuration = _builder.Build();

            // Pull the values from the appsettings.json file.
            string searchServiceName = "mythirdsearch";
            string queryApiKey = "5240DF267BD4477471B6C48D34C820AB";

            // Create a service and index client.
            _serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(queryApiKey));
            _indexClient = _serviceClient.Indexes.GetClient("azureblob-index");
        }
        private async Task<ActionResult> RunQueryAsync(SearchData model,SearchParameters parameters)
        {           
            InitSearch();        
            try
            {
                // For efficiency, the search call should be asynchronous, so use SearchAsync rather than Search.
                model.resultList = await _indexClient.Documents.SearchAsync<ResumeFile>(model.searchText, parameters);
            }
            catch (Exception ex)
            {

            }

            // Display the results.
            return View("Index", model);
        }
        public ActionResult Details(string filename)
        {
            Profile profile = ctx.Profiles.Include("Languages").Include("Skills").Include("Titles").Include("Countries").Include("Tools").Where(x => x.CvFile.fileName == filename+".pdf").FirstOrDefault();
            return PartialView(profile);
        }
    }
}