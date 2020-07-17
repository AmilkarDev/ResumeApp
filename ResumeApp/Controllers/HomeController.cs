using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using MoreLinq;
using PagedList;
using ResumeApp.Helpers;
using ResumeApp.Models;
using ResumeApp.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TikaOnDotNet.TextExtraction;
namespace ResumeApp.Controllers
{
    public class HomeController : Controller
    {
        private const string _server = "mail.ontonomia.com";
        private const string _sserver = "hp293.hostpapa.com";
        private const string _user = "tyouba@ontonomia.com";
        private const string _password = "Mta%641994";
        public HomeController() { }
        ApplicationDbContext ctx = new ApplicationDbContext();
        public HomeController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }


        // Our way to access the AplicationUser Model and apply modifications ( delete , create , edit ...)
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult OurServices()
        {
            return View();
        }
        public ActionResult Features()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UploadFiles()
        {
            //var tools = ctx.Softwares.ToList();
            //var titles = ctx.Titles.ToList();
            //var languages = ctx.Languages.ToList();
            //var skills = ctx.Skills.ToList();
            //var countries = ctx.Countries.ToList();
            //foreach (var item in tools)
            //{
            //    item.Text = item.Text.ToLower();

            //}
            //foreach (var item in titles)
            //{
            //    item.Text = item.Text.ToLower();
            //}
            //foreach (var item in languages)
            //{
            //    item.Text = item.Text.ToLower();
            //}
            //foreach (var item in skills)
            //{
            //    item.Text = item.Text.ToLower();
            //}
            //foreach (var item in countries)
            //{
            //    item.Text = item.Text.ToLower();
            //}
            //ctx.SaveChanges();

            //  ViewBag.files = ll;

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadFiles(HttpPostedFileBase[] files)
        {
            List<string> existingFiles = ctx.CvFiles.Select(x => x.fileName).ToList();
            //Ensure model state is valid  
            if (ModelState.IsValid && files != null)

                UploadFilesToAzureStorage(files);


            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    //Checking file is available to save.  
                    if (file != null && !existingFiles.Contains(file.FileName))
                    {
                        var InputFileName = System.IO.Path.GetFileName(file.FileName);
                        var ServerSavePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        //Save file info to db
                        CvFile cvfile = new CvFile { fileName = InputFileName };
                        Profile pp = await ExtractTextFromPdf(ServerSavePath);
                        pp.CvFile = cvfile;
                        ctx.Profiles.Add(pp);
                        ctx.CvFiles.Add(cvfile);
                        ctx.SaveChanges();
                        //assigning file uploaded status to ViewBag for showing message to user.  
                        ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                    }

                }
            }
            return View();
        }
        private void UploadFilesToAzureStorage(IEnumerable<HttpPostedFileBase> files)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=firstaccount;AccountKey=HG5VgrWBbgzdCXsQ1szWP1XI1XzCaHgOGbzSmo2So7wNxr19E2+EeVzVv/6l7lg6E7kJLkzSv7CjSSJVeKddJQ==;EndpointSuffix=core.windows.net");

            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer storageContainer = BlobClient.GetContainerReference("firstcontainer");

            foreach (var file in files)
            {
                if (file?.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    CloudBlockBlob blockBlob = storageContainer.GetBlockBlobReference(fileName);
                    blockBlob.UploadFromStream(file.InputStream);
                }
            }
        }
        public ActionResult DisplayFiles(string search, int? pageNumber)
        {
            var listacv = new List<CvFile>();
            listacv = ctx.CvFiles.ToList();
            var listProfiles = ctx.Profiles.ToList();
            var final = listProfiles.ToPagedList(pageNumber ?? 1, 2);
            return PartialView(final);
        }
        //public ActionResult Search(string search, int? pageNumber)
        //{
        //    //var listacv = new List<CvFile>();
        //    //listacv = ctx.CvFiles.ToList();
        //    //var listProfiles = ctx.Profiles.ToList();
        //    //var final = listProfiles.ToPagedList(pageNumber ?? 1, 2);
        //    //return PartialView(final);
        //}
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
                if (pp == null)
                {
                    return HttpNotFound();
                }
                return PartialView(pp);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
        }


        public ActionResult Valid(int Id)
        {
            if (Request.IsAjaxRequest())
            {
                var pp = ctx.Profiles.Where(c => c.Id == Id).FirstOrDefault();
                //ctx.Entry(pp).Collection(x => x.Countries).Load();
                //ctx.Entry(pp).Collection(x => x.Languages).Load();
                //ctx.Entry(pp).Collection(x => x.Skills).Load();
                //ctx.Entry(pp).Collection(x => x.Titles).Load();
                //ctx.Entry(pp).Collection(x => x.Tools).Load();
                if (pp == null)
                {
                    return HttpNotFound();
                }
                return PartialView(pp);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        [HttpPost]
        public JsonResult Valid(Profile profile)
        {
            Profile pr = ctx.Profiles.Where(x => x.Id == profile.Id).FirstOrDefault();
            pr.PhoneNumber = profile.PhoneNumber;
            pr.Nationality = profile.Nationality;
            pr.FullName = profile.FullName;
            pr.Email = profile.Email;
            pr.Validated = true;
            ctx.SaveChanges();
            return Json("enregistré avec succés ", JsonRequestBehavior.AllowGet);
        }

        //public  async Task<JsonResult> saveFile(string id)
        //{
        //        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //    string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));
        //    var fullpath = Directory.GetFiles(newPath).Where(x => x.Contains(id)).FirstOrDefault();            
        //    Uri uri = new Uri(fullpath);

        //    string filename = System.IO.Path.GetFileName(uri.LocalPath);
        //    using (WebClient myWebClient = new WebClient())
        //    {
        //        string destination = filename;
        //        await myWebClient.DownloadFileTaskAsync(uri, desktopPath+'/'+destination);
        //        return Json("download done successfully", JsonRequestBehavior.AllowGet);
        //    }
        //    //byte[] fileBytes = System.IO.File.ReadAllBytes(fullpath);            
        //    //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}


        public FileResult Download(string id)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/UploadedFiles"));
            var fullpath = Directory.GetFiles(dirInfo.FullName).Where(x => x.Contains(id)).FirstOrDefault();
            //var fullpath = fullpath + id + ".pdf";
            string contentType = string.Empty;
            contentType = "application/pdf";
            return File(fullpath, contentType, id + ".pdf");


            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=firstaccount;AccountKey=HG5VgrWBbgzdCXsQ1szWP1XI1XzCaHgOGbzSmo2So7wNxr19E2+EeVzVv/6l7lg6E7kJLkzSv7CjSSJVeKddJQ==;EndpointSuffix=core.windows.net");

            //CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
            //CloudBlobContainer storageContainer = BlobClient.GetContainerReference("firstcontainer");


            //string fileName = id+".pdf";
            //CloudBlockBlob blockBlob = storageContainer.GetBlockBlobReference(fileName);
            //blockBlob.DownloadToFile("C:\\Users\\Amilkar\\Documents\\AppResumeDocs" + fileName, FileMode.Create);



        }
        public JsonResult removeFile(int Id)
        {
            int idFile = ctx.Profiles.Where(x=>x.Id==Id).FirstOrDefault().CvFileId;
            var file = ctx.CvFiles.Single(x => x.Id == idFile);
            string id = file.fileName;
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));
            var fullpath = Directory.GetFiles(newPath).Where(x => x.Contains(id)).FirstOrDefault();
//bool poca = System.IO.File.Exists(filename);
            if (fullpath!=null)
            {
                Uri uri = new Uri(fullpath);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            
                System.IO.File.Delete(uri.LocalPath);
            }
            
            Profile pr = ctx.Profiles.Where(x => x.CvFileId == file.Id).FirstOrDefault();
            ctx.Entry(pr).Collection(s => s.Entities).Load() ;
            ctx.Entry(pr).Collection(s => s.KeyPhrases).Load();
            //foreach (var item in pr.Entities) ctx.Entities.Remove(item);
            //foreach (var item in pr.KeyPhrases) ctx.KeyPhrases.Remove(item);
            pr.Entities.ToList().ForEach(r => ctx.Entities.Remove(r));
            pr.KeyPhrases.ToList().ForEach(r => ctx.KeyPhrases.Remove(r));
            ctx.Profiles.Remove(pr);
            ctx.CvFiles.Remove(file);
            ctx.SaveChanges();
            /*************** return List of Files  *************************************************/
            List<string> ll = new List<string>();
            var k = Directory.GetFiles(newPath);
            foreach (var item in k)
            {
                var tt = item.Split('\\');
                ll.Add(tt.LastOrDefault());
            }
            ViewBag.files = ll;
            return Json("removal done successfully", JsonRequestBehavior.AllowGet);
        }

        public async Task<Profile> ExtractTextFromPdf(string path)
        {
            var sb = new StringBuilder();
            Profile pp = new Profile();
            try
            {
                //using (PdfReader reader = new PdfReader(path))
                //{
                    //StringBuilder text = new StringBuilder();
                    //List<string> ll = new List<string>();
                    //string txt = PdfTextExtractor.GetTextFromPage(reader, 1);
                    var textExtractor = new TextExtractor();
                    var wordDocContents = textExtractor.Extract(path);
                    string content = wordDocContents.Text;



                    var ll1 = content.Split('\n');
                    bool vala = false;
                    bool found = false;
                    foreach (var item in ll1)
                    {
                        if (item.Contains("Mr") || item.Contains("Nom") || item.Contains("Prénom"))
                        {
                            vala = true;
                            pp.FullName = item;
                        }
                        if (!found)
                        {
                            string st = "";
                            var b = item.Split(' ');
                            foreach (var ss in b)
                            {
                                if (ss.Contains("@") || ss.Contains("gmail") || ss.Contains("yahoo") || ss.Contains("outlook") || (ss.Contains(".com") && item.Contains("@")) || (ss.Contains(".tn") && ss.Contains("@")) || (ss.Contains(".fr") && ss.Contains("@")) || ss.Contains("email") || ss.Contains("E-mail") || ss.Contains("Email"))
                                {
                                    found = true;
                                    pp.Email = ss;
                                }
                            }
                        }
                        if (item.Contains("216") || item.Contains("Tél") || item.Contains("Téléphone") || item.Contains("+33") || item.Contains("Tel") || item.Contains("+91"))
                        {
                            string st = "";
                            var b = item.Split(' ');
                            foreach(var ss in b)
                            {
                                if (ss.Any(char.IsDigit) || ss== "–" || ss== "_"||ss=="-") st = st+' ' + ss;
                            }
                            //pp.PhoneNumber = item;
                            pp.PhoneNumber = st;
                           // Console.WriteLine("Phone number" + item);
                        }
                    }
                    
                    //var textExtractor = new TextExtractor();
                    //var wordDocContents = textExtractor.Extract(path);
                    //string content = wordDocContents.Text;

                    content = Regex.Replace(content, @"\s+", " ").ToLower();
                    int chunkSize = 5100;
                    int stringLength = content.Length;
                    List<string> strr = new List<string>();
                    for (int j = 0; j < stringLength; j += chunkSize)
                    {
                        if (j + chunkSize > stringLength) chunkSize = stringLength - j;
                        strr.Add(content.Substring(j, chunkSize));
                    }
                    pp.Entities = await Analytics.ExtractEntities( strr);
                    pp.KeyPhrases = await Analytics.ExtractKeyPhrases(strr);
                    content = content.MakeAlphaNumeric(new char[] { '-', ' ' });
                    pp.Countries = ExtractCountries(content);
                    pp.Languages = ExtractLanguages(content);
                    pp.Titles = ExtractTitles(content);
                    pp.Tools = ExtractTools( content);
                    pp.Skills = ExtractSkills( content);
                    //Removing values duplications in entities , to not have for example same profile with two entities of type Location that have the text "Marrakech"
                    pp = FixData(pp);
                    return pp;
                //}
            }
            catch (Exception e)
            {
                throw e;
            }

}
        public Profile FixData(Profile pr)
        {
            var al = pr.Entities.GroupBy(x => x.Text);
            bool count;
            foreach (var item in al)
            {

                if (item.Count() > 1)
                {
                    // I use count variable to check if it's the first element , if it's , the we keep it , we remove all the rest duplicates
                    count = false;
                    foreach (Entity ent in item)
                    {
                        if (count)
                        {
                            pr.Entities.Remove(ent);
                            //ctx.Entities.Remove(ent);
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
                            //ctx.KeyPhrases.Remove(kph);
                        }
                        newCount = true;
                    }
                }

            }




            //ctx.SaveChanges();
            return pr;
        }
        public  List<Language> ExtractLanguages(string content)
        {
            //List<string> languages = new List<string> { "italien", "Italien", "Arabe", "français", "arabe", "Français", "Anglais", "anglais", "Allemand", "Espagnol", "allemand", "espagnol" };
            List<string> languages = ctx.KnowledgeBases.Include(k => k.Languages).FirstOrDefault().Languages.Select(x=>x.Text).ToList();
            List<Language> str= new List<Language>();
            AhoCorasick.Trie triee = new AhoCorasick.Trie();
            triee.Add(languages);
            triee.Build();
            var listaa = triee.Find(content).ToList();
            foreach (string word in listaa.Distinct())
            {
                str.Add(ctx.Languages.Where(x => x.Text == word).FirstOrDefault());
            }
            return str;
        }
        public  List<Country> ExtractCountries(string content)
        {
            //List<string> countries = new List<string> { "Canada", "France", "Brésil", "Amérique", "Tunisie", "Algérie", "Maroc", "Australie", "Arabie saoudite", "Allemagne", "Suède" };

            List<string> countries = ctx.KnowledgeBases.Include(k=>k.Countries).FirstOrDefault().Countries.Select(x => x.Text).ToList();
            List<Country> str = new List<Country>();
            AhoCorasick.Trie triee = new AhoCorasick.Trie();
            triee.Add(countries);
            triee.Build();
            // Console.WriteLine("\n \n \n \n Liste des Langues \n \n \n");
            var listaa = triee.Find(content).ToList();
            foreach (string word in listaa.Distinct())
            {
                str.Add(ctx.Countries.Where(x=>x.Text==word).FirstOrDefault());
                //Console.WriteLine(word);

            }
            return str;
        }
        public  List<Skill> ExtractSkills(string content)
        {
            //List<string> countries = new List<string> { "Canada", "France", "Brésil", "Amérique", "Tunisie", "Algérie", "Maroc", "Australie", "Arabie saoudite", "Allemagne", "Suède" };

            List<string> skills = ctx.KnowledgeBases.Include(k => k.Skills).FirstOrDefault().Skills.Select(x => x.Text).ToList();
            List<Skill> str = new List<Skill>();
            AhoCorasick.Trie triee = new AhoCorasick.Trie();
            triee.Add(skills);
            triee.Build();
            var listaa = triee.Find(content).ToList();
            foreach (string word in listaa.Distinct())
            {
                str.Add(ctx.Skills.Where(x=>x.Text==word).FirstOrDefault());
            }
            return str;
        }
        public  List<Tool> ExtractTools(string content)
        {

            List<string> tools = ctx.KnowledgeBases.Include(k => k.Tools).FirstOrDefault().Tools.Select(x => x.Text).ToList();
            List<Tool> str = new List<Tool>();
            AhoCorasick.Trie triee = new AhoCorasick.Trie();
            triee.Add(tools);
            triee.Build();
            var listaa = triee.Find(content).ToList();
            foreach (string word in listaa.Distinct())
            {
                str.Add(ctx.Softwares.Where(x => x.Text == word).FirstOrDefault());
            }
            return str;
        }
        public  List<Title> ExtractTitles( string content)
        {
            //List<string> countries = new List<string> { "Canada", "France", "Brésil", "Amérique", "Tunisie", "Algérie", "Maroc", "Australie", "Arabie saoudite", "Allemagne", "Suède" };

            List<string> titles = ctx.KnowledgeBases.Include(k => k.Titles).FirstOrDefault().Titles.Select(x => x.Text).ToList();
            List<Title> str = new List<Title>();
            AhoCorasick.Trie triee = new AhoCorasick.Trie();
            triee.Add(titles);
            triee.Build();
            var listaa = triee.Find(content).ToList();
            foreach (string word in listaa.Distinct())
            {
                str.Add(ctx.Titles.Where(x => x.Text == word).FirstOrDefault());
            }
            return str;
        }
        public ActionResult listFiles(string search, int? pageNumberr)
        {
            List<string> ll = new List<string>();
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));

            var k = Directory.GetFiles(newPath);
            var listacv = new List<CvFile>();
            foreach (var item in k)
            {
                var tt = item.Split('\\');
                ll.Add(tt.LastOrDefault());
                string filename = tt.LastOrDefault();
                CvFile newfile = new CvFile { fileName = filename };
                listacv.Add(newfile);
            }
            if(search==null) return PartialView(listacv.ToPagedList(pageNumberr ?? 1, 3));
            else
            {
                listacv = listacv.Where(x => x.fileName.Contains(search) || search == null).ToList();
                return PartialView(listacv.ToPagedList(pageNumberr ?? 1, 3));
            }
            
            
        }
        [HttpGet]
        public async Task<ActionResult> listUsers()
        {
            // trick to prevent deadlocks of calling async method 
            // and waiting for on a sync UI thread.
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            var kk = await UserManager.Users.ToListAsync();
            // restore the context
            SynchronizationContext.SetSynchronizationContext(syncContext);

            return PartialView(kk);
        }
        public ActionResult listTemplates()
        {
            return View();
        }
        
        public  ActionResult Recipients(string search, int? pageNumber,int? pageNumberr)
        {
            var listUsers =  UserManager.Users.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                listUsers = listUsers.Where(x =>x.FullName!=null && x.FullName.Contains(search)).ToList();
            }
            var final = listUsers.ToPagedList(pageNumber ?? 1, 2);
            return PartialView(final);
        }
        public async Task<ActionResult> Mailing(string search, int? pageNumber, int? pageNumberr)
        {
            return View();
        }
       public JsonResult SendEmail(FullEmail fullEmail)
       {
            FullEmail varta = fullEmail;
            try
            {
                //Fetching Email Body Text from EmailTemplate File.  
                //string filePath = System.IO.Path.Combine(Server.MapPath("~/EmailTemplates/email.html"));
                //StreamReader str = new StreamReader(filePath);
                //string mailText = str.ReadToEnd();
                //str.Close();
               
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient(_server);
                mail.IsBodyHtml = true;
                mail.Body = fullEmail.EmailContent ;
                mail.From = new MailAddress("tyouba@ontonomia.com");
                foreach (var item in fullEmail.Recipients)
                {
                    mail.To.Add(item);
                }
                foreach (var item in fullEmail.ccrecipients)
                {
                    mail.CC.Add(item);
                }
                foreach (var item in fullEmail.Attachments)
                {
                    
                    string fullpath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"+item));
                    //var fullpath = Directory.GetFiles(newPath).Where(x => x.Contains(id)).FirstOrDefault();
                    //bool poca = System.IO.File.Exists(filename);
                    if (fullpath != null)
                    {
                        Uri uri = new Uri(fullpath);
                        //string filename = System.IO.Path.GetFileName(uri.LocalPath);

                        // System.IO.File.Delete(uri.LocalPath);
                        mail.Attachments.Add(new Attachment(fullpath));
                    }

                    //mail.Attachments.Add(new Attachment("UploadedFiles/" + item.Trim()));
                }
                mail.Subject = fullEmail.Subject;
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(_user, _password);
                //smtpServer.EnableSsl = true;
                smtpServer.Timeout=100000;
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                
            }




            return Json("successful Result ", JsonRequestBehavior.AllowGet);

        }
    }
}