using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNet.Identity.Owin;
using ResumeApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

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
            List<string> ll = new List<string>();
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));

            var k = Directory.GetFiles(newPath);
            foreach (var item in k)
            {
                var tt = item.Split('\\');
                ll.Add(tt.LastOrDefault());
            }
            ViewBag.files = ll;
            
            return View();
        }
        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files)
        {

            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var InputFileName = System.IO.Path.GetFileName(file.FileName);
                        var ServerSavePath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        //assigning file uploaded status to ViewBag for showing message to user.  
                        ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                    }

                }
            }
            //return View();
            /*************** return List of Files  *************************************************/
            List<string> ll = new List<string>();
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));

            var k = Directory.GetFiles(newPath);
            foreach (var item in k)
            {
                var tt = item.Split('\\');
                ll.Add(tt.LastOrDefault());
            }
            ViewBag.files = ll;
            return View();
        }

        public ActionResult Details(string id)
        {
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));
            var k = Directory.GetFiles(newPath).Where(x=>x.Contains(id)).FirstOrDefault() ;


            if (Request.IsAjaxRequest())
            {


                if (id == "None")
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Profile profile = ExtractTextFromPdf(k);
                
                if (profile == null)
                {
                    return HttpNotFound();
                }
                return PartialView(profile);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
        }

        public  async Task<JsonResult> saveFile(string id)
        {
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));
            var fullpath = Directory.GetFiles(newPath).Where(x => x.Contains(id)).FirstOrDefault();            
            Uri uri = new Uri(fullpath);

            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            using (WebClient myWebClient = new WebClient())
            {
                string destination = "C:/Users/Amilkar/Documents/AppResumeDocs/" + filename;
                 await myWebClient.DownloadFileTaskAsync(uri, destination);
                return Json("download done successfully", JsonRequestBehavior.AllowGet);
            }
            //byte[] fileBytes = System.IO.File.ReadAllBytes(fullpath);            
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

     
        public FileResult Download(string id)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/UploadedFiles"));
            var fullpath = Directory.GetFiles(dirInfo.FullName).Where(x => x.Contains(id)).FirstOrDefault();
            //int CurrentFileID = Convert.ToInt32(FileID);
            //var filesCol = obj.GetFiles();
            //string CurrentFileName = (from fls in filesCol
            //                          where fls.FileId == CurrentFileID
            //                          select fls.FilePath).First();
            string contentType = string.Empty;            
            contentType = "application/pdf";
            return File(fullpath, contentType, id+".pdf");
        }
        public JsonResult removeFile(string id)
        {
            string newPath = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/"));
            var fullpath = Directory.GetFiles(newPath).Where(x => x.Contains(id)).FirstOrDefault();
            Uri uri = new Uri(fullpath);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            bool poca = System.IO.File.Exists(filename);
            System.IO.File.Delete(uri.LocalPath);
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

        public static Profile ExtractTextFromPdf(string path)
        {
            var sb = new StringBuilder();
            Profile pp = new Profile();
            try
            {
                using (PdfReader reader = new PdfReader(path))
                {
                    StringBuilder text = new StringBuilder();
                    List<string> ll = new List<string>();
                    //for (int i = 1; i <= reader.NumberOfPages; i++)
                    //{
                    //text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    string txt = PdfTextExtractor.GetTextFromPage(reader, 1);
                    var ll1 = txt.Split('\n');
                    bool vala = false;
                    // variable to detect whther first email address has been found 
                    bool found = false;
                    foreach (var item in ll1)
                    {
                        if (item.Contains("Mr") || item.Contains("Nom") || item.Contains("Prénom"))
                        {
                            vala = true;
                            pp.FullName = item;
                            // Console.WriteLine("First & Last Name "+item);
                        }
                        if (!found)
                        {
                            if (item.Contains("@") || item.Contains("gmail") || item.Contains("yahoo") || item.Contains("outlook") || (item.Contains(".com") && item.Contains("@")) || (item.Contains(".tn") && item.Contains("@")) || (item.Contains(".fr") && item.Contains("@")))
                            {
                                found = true;
                                pp.Email = item;
                                //Console.WriteLine("Email address " + item);
                            }
                        }
                        if (item.Contains("216") || item.Contains("Tél") || item.Contains("Téléphone") || item.Contains("+33") || item.Contains("Tel") || item.Contains("+91"))
                        {
                            pp.PhoneNumber = item;
                            Console.WriteLine("Phone number" + item);
                        }
                        //PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
                        //var phoneNumber = phoneUtil.FindNumbers(item,null);
                        //var tt = phoneUtil.FindNumbers(item,"TN");
                        //var kk = true;
                        //foreach (var ita in phoneNumber)
                        //{
                        //    Console.WriteLine(ita.ToString());
                        //    Console.WriteLine(tt.ToString());
                        //}
                        //foreach (var ita in tt)
                        //{
                        //    Console.WriteLine(ita.ToString());

                        //}
                        //var count = item.Count(char.IsDigit);
                        //if (count > 6)
                        //{
                        //    Console.WriteLine("Phone Number " + item);
                        //}

                    }
                    //if (vala == false)
                    //{
                    //Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&& présentation générale &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                    //    Console.WriteLine(ll1[0]);
                    //    Console.WriteLine(ll1[1]);
                    //    Console.WriteLine(ll1[2]);
                    //    Console.WriteLine(ll1[3]);
                    //}
                    //ll.Add(txt);
                    text.Append(txt);
                    //}

                    // return text.ToString();
                    return pp;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public ActionResult listFiles()
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
            return PartialView(listacv);
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
        
        public async Task<ActionResult> Mailing()
        {
            var kk = await UserManager.Users.ToListAsync();

            return View(kk);
        }
       public JsonResult SendEmail(FullEmail fullEmail)
       {
            FullEmail varta = fullEmail;
            try
            {
                //Fetching Email Body Text from EmailTemplate File.  
                string filePath = System.IO.Path.Combine(Server.MapPath("~/EmailTemplates/email.html"));
                StreamReader str = new StreamReader(filePath);
                string mailText = str.ReadToEnd();
                str.Close();
               
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient(_server);
                mail.IsBodyHtml = true;
                mail.Body = mailText;
                mail.From = new MailAddress("tyouba@ontonomia.com");
                mail.To.Add("bh.imen@ontonomia.com");
                mail.Subject = "Test Mail";
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