using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ResumeApp.Models;
using ResumeApp.Models.Helpers;

namespace ResumeApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KnowledgeBasesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KnowledgeBases
        public ActionResult Index()
        {
            return View(db.KnowledgeBases.ToList());
        }
        public ActionResult ListKb()
        {
            var kbs = db.KnowledgeBases.ToList();
            return PartialView(kbs);
        }
        // GET: KnowledgeBases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeBase knowledgeBase = db.KnowledgeBases.Find(id);
            if (knowledgeBase == null)
            {
                return HttpNotFound();
            }
            return View(knowledgeBase);
        }

        // GET: KnowledgeBases/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KnowledgeBases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] KnowledgeBase knowledgeBase)
        {
            if (ModelState.IsValid)
            {
                db.KnowledgeBases.Add(knowledgeBase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(knowledgeBase);
        }

        // GET: KnowledgeBases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeBase knowledgeBase = db.KnowledgeBases.Find(id);
            if (knowledgeBase == null)
            {
                return HttpNotFound();
            }
            return View(knowledgeBase);
        }

        // POST: KnowledgeBases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] KnowledgeBase knowledgeBase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(knowledgeBase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(knowledgeBase);
        }

        // GET: KnowledgeBases/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    KnowledgeBase knowledgeBase = db.KnowledgeBases.Find(id);
        //    if (knowledgeBase == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(knowledgeBase);
        //}

        // POST: KnowledgeBases/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    KnowledgeBase knowledgeBase = db.KnowledgeBases.Find(id);
        //    db.KnowledgeBases.Remove(knowledgeBase);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult List()
        {
            var kbs = db.KnowledgeBases.ToList();
            return Json(kbs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(KnowledgeBase kb)
        {
            db.KnowledgeBases.Add(kb);
            db.SaveChanges();
            return Json("successfully created", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetbyID(int ID)
        {
            var kb = db.KnowledgeBases.Where(x=>x.Id==ID).Include(p=>p.Countries).FirstOrDefault();
            db.Entry(kb).Collection(x => x.Skills).Load();
            db.Entry(kb).Collection(x => x.Languages).Load();
            db.Entry(kb).Collection(x => x.Tools).Load();
            db.Entry(kb).Collection(x => x.Titles).Load();
            var list = JsonConvert.SerializeObject(kb, Formatting.None, new JsonSerializerSettings()
                                                                                {
                                                                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                                                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(KnowledgeBase kb)
        {
            var kbi = db.KnowledgeBases.Where(x => x.Id == kb.Id).FirstOrDefault();
            kbi.Name = kb.Name;
            kbi.Description = kb.Description;
            db.SaveChanges();
            return Json("Knowledge Base updated successfully", JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult Delete(int ID)
        {
            var kb = db.KnowledgeBases.Where(c => c.Id == ID).FirstOrDefault();
            db.KnowledgeBases.Remove(kb);
            db.SaveChanges();
            return Json("Knowledge Base removed successfully", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveCountries(KBviewModel data)
        {
            List<string> existingCountries = new List<string>();
            var cc = db.KnowledgeBases.Single(x => x.Id == data.KbId);
            db.Entry(cc).Collection(x => x.Countries).Load();
            var mainCountries = db.Countries.Select(c => c.Text).ToList();
            List<string> newList = new List<string>();
            newList = lowerStringContent(data.Content);
            //get countries of the specified knowledgeBase 
            foreach (var country in cc.Countries)
            {
                existingCountries.Add(country.Text);
            }
            foreach(var str in existingCountries)
            {
                if (!newList.Contains(str))
                {
                    var country = db.Countries.Where(c => c.Text == str).FirstOrDefault();
                    cc.Countries.Remove(country);
                }
            }
            var newCountries = newList.Distinct();
            foreach(var item in newCountries)
            {  
                if (!existingCountries.Contains(item))
                {
                    if (mainCountries.Contains(item))
                    {
                        var country = db.Countries.Where(c => c.Text == item).FirstOrDefault();
                        cc.Countries.Add(country);
                    }

                    else
                    {
                        cc.Countries.Add(new Country { Text = item });
                    }
                    
                }

            }
            db.SaveChanges();
            return Json("successfully registered");
        }
        [HttpPost]
        public JsonResult SaveLanguages(KBviewModel data)
        {
            List<string> existingLanguages = new List<string>();
            var cc = db.KnowledgeBases.Single(x => x.Id == data.KbId);
            db.Entry(cc).Collection(x => x.Languages).Load();
            var mainLanguages = db.Countries.Select(c => c.Text).ToList();
            List<string> newList = new List<string>();
            newList = lowerStringContent(data.Content);
            //get countries of the specified knowledgeBase 
            foreach (var lang in cc.Languages)
            {
                existingLanguages.Add(lang.Text);
            }
            foreach (var str in existingLanguages)
            {
                if (!newList.Contains(str))
                {
                    var lang = db.Languages.Single(c => c.Text == str);
                    cc.Languages.Remove(lang);
                }
            }
            foreach (var item in newList.Distinct())
            {
                if (!existingLanguages.Contains(item))
                {
                    if (mainLanguages.Contains(item))
                    {
                        var lang = db.Languages.Where(c => c.Text == item).FirstOrDefault();
                        cc.Languages.Add(lang);
                    }

                    else
                    {
                        cc.Languages.Add(new Language { Text = item });
                    }

                }

            }
            db.SaveChanges();
            return Json("successfully registered");
        }

        public List<string> lowerStringContent(string[] str)
        {
            List<string> ll = new List<string>();
            foreach(var item in str)
            {
               
                ll.Add(item.ToLower());
            }
            return ll;
        }

        [HttpPost]
        public JsonResult SaveSkills(KBviewModel data)
        {
            List<string> existingSkills = new List<string>();
            var cc = db.KnowledgeBases.Single(x => x.Id == data.KbId);
            db.Entry(cc).Collection(x => x.Skills).Load();
            var mainSkills = db.Skills.Select(c => c.Text).ToList();
            List<string> newList = new List<string>();
            newList = lowerStringContent(data.Content);
            //get countries of the specified knowledgeBase 
            foreach (var skill in cc.Skills)
            {
                existingSkills.Add(skill.Text);
            }
            foreach (var str in existingSkills)
            {
                if (!newList.Contains(str))
                {
                    var skill = db.Skills.Single(c => c.Text == str);
                    cc.Skills.Remove(skill);
                }
            }
            foreach (var item in newList.Distinct())
            {
                if (!existingSkills.Contains(item))
                {
                    if (mainSkills.Contains(item))
                    {
                        var skill = db.Skills.Where(c => c.Text == item).FirstOrDefault();
                        cc.Skills.Add(skill);
                    }

                    else
                    {
                        cc.Skills.Add(new Skill { Text = item });
                    }

                }

            }
            db.SaveChanges();
            return Json("successfully registered");
        }


        [HttpPost]
        public JsonResult SaveTools(KBviewModel data)
        {
            List<string> existingTools = new List<string>();
            var cc = db.KnowledgeBases.Single(x => x.Id == data.KbId);
            db.Entry(cc).Collection(x => x.Tools).Load();
            var mainTools = db.Softwares.Select(c => c.Text).ToList();
            //get countries of the specified knowledgeBase 
            List<string> newList = new List<string>();
            newList = lowerStringContent(data.Content);
            foreach (var skill in cc.Skills)
            {
                existingTools.Add(skill.Text);
            }
            foreach (var str in existingTools)
            {
                if (!newList.Contains(str))
                {
                    var skill = db.Skills.Single(c => c.Text == str);
                    cc.Skills.Remove(skill);
                }
            }
            foreach (var item in newList.Distinct())
            {
                if (!existingTools.Contains(item))
                {
                    if (mainTools.Contains(item))
                    {
                        var tool = db.Softwares.Where(c => c.Text == item).FirstOrDefault();
                        cc.Tools.Add(tool);
                    }
                    else
                    {
                        cc.Tools.Add(new Tool { Text = item });
                    }
                }
            }
            db.SaveChanges();
            return Json("successfully registered");
        }

        [HttpPost]
        public JsonResult SaveTitles(KBviewModel data)
        {
            List<string> existingTitles = new List<string>();
            var cc = db.KnowledgeBases.Single(x => x.Id == data.KbId);
            db.Entry(cc).Collection(x => x.Titles).Load();
            var mainTitles = db.Titles.Select(c => c.Text).ToList();
            List<string> newList = new List<string>();
            newList = lowerStringContent(data.Content);
            //get countries of the specified knowledgeBase 
            foreach (var title in cc.Titles)
            {
                existingTitles.Add(title.Text);
            }
            foreach (var str in existingTitles)
            {
                if (!newList.Contains(str))
                {
                    var title = db.Titles.Single(c => c.Text == str);
                    cc.Titles.Remove(title);
                }
            }
            foreach (var item in newList.Distinct())
            {
                item.ToLower();
                if (!existingTitles.Contains(item))
                {
                    if (mainTitles.Contains(item))
                    {
                        var title = db.Titles.Where(c => c.Text == item).FirstOrDefault();
                        cc.Titles.Add(title);
                    }

                    else
                    {
                        cc.Titles.Add(new Title { Text = item });
                    }

                }

            }
            db.SaveChanges();
            return Json("successfully registered");
        }
    }
}
