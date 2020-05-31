using Microsoft.AspNet.Identity.Owin;
using ResumeApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ResumeApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IdentityController : Controller
    {

        public IdentityController()
        {
        }
        ApplicationDbContext ctx = new ApplicationDbContext();
        public IdentityController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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



        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }
        /// <summary>
        /// Afficher les détails d'un utilisateur spécifique
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            return PartialView(user);
        }





        /// <summary>
        /// Edit is the method the admin use in order to modify a user access level , so to give him new access or remove his access to some
        /// space of the app ( for example allow admin access and remove Service provider Access)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            return PartialView(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                // Include the Addresss info:
                UserName = user.UserName,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                PhotoLink = user.PhotoLink,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }



        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,FullName,UserName,Id,StreetAddress,City,State,PostalCode")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.FullName = editUser.FullName;
                user.UserName = editUser.UserName;
                user.Email = editUser.Email;
                user.StreetAddress = editUser.StreetAddress;
                user.City = editUser.City;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> EditUser( EditUserViewModel editUser,  string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return Json("No existing user with the specified Id", JsonRequestBehavior.AllowGet);
                }
                user.FullName = editUser.FullName;
                user.UserName = editUser.UserName;
                user.Email = editUser.Email;
                user.StreetAddress = editUser.StreetAddress;
                user.City = editUser.City;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return Json("We Encountered an Error , please retry Late", JsonRequestBehavior.AllowGet);
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return Json("We Encountered an Error , please retry Later",JsonRequestBehavior.AllowGet);
                }
                return Json("User Successfully Updated", JsonRequestBehavior.AllowGet);
            }
            return Json("We Encountered an Error , please retry Later", JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> RemoveUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json("We encountered an error removing the specified user! Please try again later !");
            }
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.ToArray<string>());
            if (result.Succeeded)
            {
                var resulta =  UserManager.DeleteAsync(user);
            }
            return Json("User Successfully Removed !");
        }
    }
}