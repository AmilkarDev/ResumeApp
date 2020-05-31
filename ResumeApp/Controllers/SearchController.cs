using PagedList;
using ResumeApp.Helpers;
using ResumeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ResumeApp.Controllers
{
    public class SearchController : Controller
    {

        public SearchController() { }
        ApplicationDbContext ctx = new ApplicationDbContext();
        // GET: Search
        public ActionResult Index()
        {
            Session["searchModel"] = new SearchModel() ;
            return View();
        }
        public Profile FixData(Profile pr)
        {
            var al = pr.Entities.GroupBy(x => x.Text);
            bool count;
            foreach (var item in al)
            {
                
                if (item.Count() > 1) {
                    // I use count variable to check if it's the first element , if it's , the we keep it , we remove all the rest duplicates
                    count = false;
                    foreach (Entity ent in item)
                    {
                        if (count)
                        {
                            pr.Entities.Remove(ent);
                            ctx.Entities.Remove(ent);
                        }
                        count = true;
                    }
                }
                
            }

            var kp = pr.KeyPhrases.GroupBy(x => x.Text);
            bool newCount;
            foreach (var item in kp)
            {

                if (item.Count() > 1)
                {
                    // I use count variable to check if it's the first element , if it's , the we keep it , we remove all the rest duplicates
                    newCount = false;
                    foreach (KeyPhrase kph in item)
                    {
                        if (newCount)
                        {
                            pr.KeyPhrases.Remove(kph);
                            ctx.KeyPhrases.Remove(kph);
                        }
                        newCount = true;
                    }
                }

            }




            //ctx.SaveChanges();
            return pr;
        }
        public ActionResult Details(int Id)
        {
            if (Request.IsAjaxRequest())
            {
                var pp = ctx.Profiles.Where(c => c.Id == Id).FirstOrDefault();
                ctx.Entry(pp).Collection(x => x.Countries).Load();
                ctx.Entry(pp).Collection(x => x.Languages).Load();
                ctx.Entry(pp).Collection(x => x.Skills).Load();
                ctx.Entry(pp).Collection(x => x.Titles).Load();
                ctx.Entry(pp).Collection(x => x.Tools).Load();
                ctx.Entry(pp).Collection(x => x.Entities).Load();
                ctx.Entry(pp).Collection(x => x.KeyPhrases).Load();
                pp = FixData(pp);
                ctx.SaveChanges();
                if (pp == null)
                {
                    return HttpNotFound();
                }
                return PartialView(pp);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        }
        public ActionResult DisplayFiles(int? pageNumber)
        {
            List<Profile> listProfiles = new List<Profile>();
            SearchModel search = Session["searchModel"] as SearchModel;
            if (search != null)
            {
                listProfiles = SearchOp();
            }
            else
            {
                listProfiles = ctx.Profiles.ToList();
            }
            var final = listProfiles.ToPagedList(pageNumber ?? 1, 2);
            return PartialView(final);
        }
        public List<Profile> SearchOp()
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            var listacv = new List<CvFile>();
            listacv = ctx.CvFiles.ToList();
            var listProfiles = ctx.Profiles.Include("Skills").Include("Languages").Include("Titles").Include("Countries").Include("Tools").Include("Entities").ToList();
            foreach (var item in search.SkillIds)
            {
                Skill ss = ctx.Skills.Where(s => s.Id == item).FirstOrDefault();
                listProfiles = listProfiles.Where(x => x.Skills.Contains(ss)).ToList();
            }
            foreach (var item in search.TitleIds)
            {
                Title title = ctx.Titles.Where(s => s.Id == item).FirstOrDefault();
                listProfiles = listProfiles.Where(x => x.Titles.Contains(title)).ToList();
            }
            foreach (var item in search.ToolIds)
            {
                Tool tool = ctx.Softwares.Where(s => s.Id == item).FirstOrDefault();
                listProfiles = listProfiles.Where(x => x.Tools.Contains(tool)).ToList();
            }
            foreach (var item in search.LanguageIds)
            {
                Language language = ctx.Languages.Where(s => s.Id == item).FirstOrDefault();
                listProfiles = listProfiles.Where(x => x.Languages.Contains(language)).ToList();
            }
            foreach (var item in search.CountryIds)
            {
                Country country = ctx.Countries.Where(s => s.Id == item).FirstOrDefault();
                listProfiles = listProfiles.Where(x => x.Countries.Contains(country)).ToList();
            }
            foreach (var item in search.profileIds)
            {
                listProfiles = listProfiles.Where(x => x.Id == item).ToList();
            }
            foreach (var item in search.EntityIds)
            {

                string text = ctx.Entities.Where(s => s.Id == item).FirstOrDefault().Text;
                List<Entity> ents = ctx.Entities.Where(x => x.Text == text).ToList();
                //Here we are implementing an OR Operation 
                var pros = new List<Profile>();
                foreach (var ent in ents)
                {
                    if(ent.Profile!=null)
                    pros.Add(ent.Profile);
                }

                listProfiles = listProfiles.Intersect(pros).ToList();
            }
            foreach (var item in search.KeyPhrasesIds)
            {
                KeyPhrase kp = ctx.KeyPhrases.Where(s => s.Id == item).FirstOrDefault();
                listProfiles = listProfiles.Where(x => x.KeyPhrases.Contains(kp)).ToList();
            }
            return listProfiles;
        }
        public ActionResult SearchKeyPhrases()
        {
            var kPhrases = ctx.KeyPhrases.ToList();
            var items = new List<Item>();
            foreach (var item in kPhrases)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchSkills()
        {
            //var skills = ctx.Skills.Where(p => p.Text.Contains(term)).Select(p => p.Text).ToList();
            var skills = ctx.Skills.ToList();
            var items = new List<Item>();
            foreach (var item in skills)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchTools(string term)
        {
            var tools = ctx.Softwares.ToList();
            var items = new List<Item>();
            foreach (var item in tools)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchTitles(string term)
        {
            var titles = ctx.Titles.ToList();
            var items = new List<Item>();
            foreach (var item in titles)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchCountries(string term)
        {
            var countries = ctx.Countries.ToList();
            var items = new List<Item>();
            foreach (var item in countries)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchOrganizations(string term)
        {
            var organizations = ctx.Entities.Where(x => x.Type == "Organization").ToList();
            var items = new List<Item>();
            foreach (var item in organizations)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLocations(string term)
        {
            var titles = ctx.Entities.Where(x => x.Type == "Location").ToList();
            var items = new List<Item>();
            foreach (var item in titles)
            {
                items.Add(new Item { value = item.Text, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchEmails(string term)
        {
            var profiles = ctx.Profiles.ToList();
            var items = new List<Item>();
            foreach (var item in profiles)
            {
                items.Add(new Item { value = item.Email, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchNames(string term)
        {
            var profiles = ctx.Profiles.ToList();
            var items = new List<Item>();
            foreach (var item in profiles)
            {
                if (item.FullName != null)
                    items.Add(new Item { value = item.FullName, data = item.Id });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLanguages(string term)
        {
            var items = ctx.Languages.Select(r => new Item { value = r.Text, data = r.Id }).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
            //var items = new List<Item>();
            //foreach (var item in languages)
            //{
            //    items.Add(new Item { value = item.Text, data = item.Id });
            //}

        }


        public JsonResult FixSearchkeyPhrases(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.KeyPhrasesIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }

        public JsonResult FixSearchskills(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.SkillIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult FixSearchTools(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.ToolIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult FixSearchTitles(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.TitleIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }


        public JsonResult FixSearchCountries(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.CountryIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult FixSearchOrganizations(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.EntityIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult FixSearchLocations(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;           
            search.EntityIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }




        public JsonResult FixSearchEmails(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.profileIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult FixSearchNames(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.profileIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult FixSearchLanguages(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.LanguageIds.Add(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeLanguage(int id)
        {
            //try
            //{
                SearchModel search = Session["searchModel"] as SearchModel;
                search.LanguageIds.Remove(id);
                return Json("successfully updated", JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return Json("encounter an error", JsonRequestBehavior.AllowGet);
            //}

        }
        public JsonResult removeTool(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.ToolIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeKeyPhrase(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.KeyPhrasesIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeTitle(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.TitleIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeSkill(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.SkillIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeCountry(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.CountryIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeLocation(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.EntityIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeName(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.profileIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeEmail(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.profileIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeOrganization(int id)
        {
            SearchModel search = Session["searchModel"] as SearchModel;
            search.EntityIds.Remove(id);
            return Json("successfully updated", JsonRequestBehavior.AllowGet);
        }
    }

    public class Item
    {
        public string value { get; set; }
        public int data { get; set; }
    }
}