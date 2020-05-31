﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using ResumeApp.Models;
namespace ResumeApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public RolesController()
        {
        }

        public RolesController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
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

        /// <summary>
        /// get list of roles
        /// </summary>
        /// <returns></returns>
        // GET: /Roles/
        [HttpGet]
        public ActionResult Index()
        {
            var mm = RoleManager.Roles;
            return View(RoleManager.Roles);
        }
        public ActionResult listRoles()
        {
            var mm = RoleManager.Roles;
            return PartialView(mm);
        }

        public JsonResult GetbyID(string ID)
        {
            ApplicationRole mm = RoleManager.Roles.Where(role=>role.Id==ID).FirstOrDefault()  ;
            var rola = JsonConvert.SerializeObject(mm, Formatting.None);
            return Json(rola,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get the details of a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Roles/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        /// <summary>
        /// add new role
        /// </summary>
        /// <returns></returns>
        // GET: /Roles/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name);
                role.Description = roleViewModel.Description;
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return Json("Encountered an error creating a new role", JsonRequestBehavior.AllowGet);
                }
                return Json("Role created successfully ", JsonRequestBehavior.AllowGet);
            }
            return Json("Encountered an error creating a new role", JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Modify a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Roles/Edit/Admin
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            // Update the new Description property for the ViewModel:
            roleModel.Description = role.Description;
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit([Bind(Include = "Name,Id,Description")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                // Update the new Description property:
                role.Description = roleModel.Description;
                await RoleManager.UpdateAsync(role);
                return Json("Role Updated Successfully ",JsonRequestBehavior.AllowGet);
            }
            return Json("Failed to update the specified role , please try again later ", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Roles/Delete/5
        //[HttpGet]
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var role = await RoleManager.FindByIdAsync(id);
        //    if (role == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(role);
        //}

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return Json("We encountered an error removing the specified role ! Please try again later ",JsonRequestBehavior.AllowGet);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return Json("We encountered an error removing the specified role ! Please try again later ", JsonRequestBehavior.AllowGet);
                }
                IdentityResult result;

                    result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return Json("We encountered an error removing the specified role ! Please try again later ", JsonRequestBehavior.AllowGet);
                }
                return Json("Role removed successfully !!! ", JsonRequestBehavior.AllowGet);
            }
            return Json("We encountered an error removing the specified role ! Please try again later ", JsonRequestBehavior.AllowGet);
        }
    }
}
