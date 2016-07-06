using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TransformationTimelineTool.ViewModels;
using System.Threading;
using TransformationTimelineTool.Exceptions;

namespace TransformationTimelineTool.Controllers
{
    [RoutePrefix("Utilisateurs-Users")]
    [Route("{action=index}")]
    public class UsersController : BaseController
    {
        private TimelineContext db = new TimelineContext();
        private UserManager<User> userManager;
        RoleManager<IdentityRole> roleManager;

        public UsersController()
        {
            userManager = new UserManager<User>(new UserStore<User>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }


        // GET: Users
        public ActionResult Index()
        {
            UserViewModel VMUser = new UserViewModel();
            VMUser.Admins = new List<User>();
            VMUser.Approvers = new List<User>();
            VMUser.Editors = new List<User>();
            foreach (var user in db.Users.ToList<User>())
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                    VMUser.Admins.Add(user);
                if (userManager.IsInRole(user.Id, "Approver"))
                    VMUser.Approvers.Add(user);
                if (userManager.IsInRole(user.Id, "Editor"))
                    VMUser.Editors.Add(user);
            }
            VMUser.Admins = VMUser.Admins.OrderBy(u => u.UserName).ToList<User>();
            VMUser.Approvers = VMUser.Approvers.OrderBy(u => u.UserName).ToList<User>();
            VMUser.Editors = VMUser.Editors.OrderBy(u => u.UserName).ToList<User>();
            return View(VMUser);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserViewModel VMUser = new UserViewModel(db.Users.Find(id), userManager);
            if (userManager.IsInRole(id, "Approver"))
            {
                VMUser.Editors = db.Users.Where(u => u.ApproverID == id).ToList<User>();
            }
            User user = db.Users.Find(id);
            VMUser.User = user;
            ViewBag.Roles = String.Join(" - ", userManager.GetRoles(user.Id));
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(VMUser);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin,")]
        [Route("Creer-Create")]
        public ActionResult Create()
        {
            var userViewModel = new UserViewModel();

            userViewModel.PopulatedInitiatives = PopulateUserInitiativesData(new User());
            userViewModel.PopulatedRoles = PopulateUserRolesData(new User());
            userViewModel.ApproverSelect = new SelectList(Utils.GetApprover(), "Id", "UserName");

            return View(userViewModel);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,")]
        [Route("Creer-Create")]
        public ActionResult Create(UserViewModel userViewModel,
            string[] selectedInitiatives,
            string[] selectedRoles)
        {
            var userToAdd = new User();

            userToAdd.Initiatives = new List<Initiative>();

            var selectedInitiativeHS = new HashSet<string>(selectedInitiatives);
            foreach (var initiative in db.Initiatives)
            {
                if (selectedInitiativeHS.Contains(initiative.ID.ToString()))
                {
                    userToAdd.Initiatives.Add(initiative);
                }
            }

            userViewModel.PopulatedRoles = PopulateUserRolesData(userToAdd, selectedRoles);
            userViewModel.PopulatedInitiatives = PopulateUserInitiativesData(userToAdd);
            userViewModel.ApproverSelect = new SelectList(Utils.GetApprover(), "Id", "UserName");

            try
            {
                string username = Utils.GetUsernameFromEmail(userViewModel.User.Email);
                if (username != "")
                {
                    userToAdd.UserName = username;
                    userToAdd.Email = userViewModel.User.Email;
                    userToAdd.ApproverID = userViewModel.User.ApproverID;
                    if (userManager.FindByName(username) != null)
                    {
                        throw new UserException("This user already exists in our database");
                    }
                    var myResult = userManager.Create(userToAdd, "password");

                    if (myResult.Succeeded)
                    {
                        var result = userManager.AddToRoles(userToAdd.Id, selectedRoles);
                    }
                    
                    return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
                }
            }
            catch (NullReferenceException nre)
            {
                ModelState.AddModelError(string.Empty, "We were unable to find the user with the provided email.");
            }
            catch (UserException uex)
            {
                ModelState.AddModelError(string.Empty, uex.Message);
            }

            return View(userViewModel);
        }

        // GET: Users/Edit/5
        [Route("Modifier-Edit")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userViewModel = new UserViewModel();

            User user = db.Users.Find(id);

            userViewModel.User = user;
            userViewModel.PopulatedRoles = PopulateUserRolesData(user);
            userViewModel.PopulatedInitiatives = PopulateUserInitiativesData(user);
            userViewModel.ApproverSelect = new SelectList(Utils.GetApprover(), "Id", "UserName");

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(userViewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Modifier-Edit")]
        public ActionResult Edit(UserViewModel userViewModel,
            string[] selectedInitiatives,
            string[] selectedRoles)
        {

            if (userViewModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userToUpdate = db.Users
            .Where(u => u.Id == userViewModel.User.Id)
            .Single();

            //string username = Utils.GetUsernameFromEmail(user.Email);

            if (ModelState.IsValid)
            {
                userToUpdate.UserName = userViewModel.User.UserName;
                userToUpdate.Email = userViewModel.User.Email;
                userToUpdate.ApproverID = userViewModel.User.ApproverID;

                UpdateUserInitiatives(selectedInitiatives, userToUpdate);
                UpdateUserRoles(selectedRoles, userToUpdate);

                var myResult = userManager.Update(userToUpdate);


                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }
            return View(userViewModel);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin,")]
        [Route("Supprimer-Delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            ViewBag.Roles = String.Join(" - ", userManager.GetRoles(user.Id));
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,")]
        [Route("Supprimer-Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                roleManager.Dispose();
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<InitiativeData> PopulateUserInitiativesData(User user)
        {
            var allInitiatives = db.Initiatives;
            HashSet<int> userInitiatives;

            if(user.Initiatives == null)
            {
                userInitiatives = new HashSet<int>();
            }
            else
            {
                userInitiatives = new HashSet<int>(user.Initiatives.Select(i => i.ID));
            }
            var viewModel = new List<InitiativeData>();

            foreach (var initiative in allInitiatives)
            {
                viewModel.Add(new InitiativeData

                {
                    ID = initiative.ID,
                    NameE = initiative.NameE,
                    NameF = initiative.NameF,
                    Flag = userInitiatives.Contains(initiative.ID)
                });
            }

            return viewModel;
        }
        private List<RolesData> PopulateUserRolesData (User user, string[] selectedRoles)
        {
            var allRoles = db.Roles;
            var viewModel = new List<RolesData>();
            foreach (var role in allRoles)
            {
                viewModel.Add(new RolesData
                {
                    ID = role.Id,
                    Name = role.Name,
                    Flag = selectedRoles.Contains(role.Name)
                });
            }
            return viewModel;
        }
        private List<RolesData> PopulateUserRolesData(User user)
        {
            var allRoles = db.Roles;
            var userRoles = new HashSet<string>(user.Roles.Select(r => r.RoleId));
            var viewModel = new List<RolesData>();

            foreach (var role in allRoles)
            {
                viewModel.Add(new RolesData
                {
                    ID = role.Id,
                    Name = role.Name,
                    Flag = userRoles.Contains(role.Id)
                });
            }

            return viewModel;
        }

        private void UpdateUserRoles(string[] selectedRoles, User userToUpdate)
        {
            userManager.RemoveFromRoles(userToUpdate.Id, userManager.GetRoles(userToUpdate.Id).ToArray());
            if (selectedRoles != null)
            {
                userManager.AddToRoles(userToUpdate.Id, selectedRoles);
            }
        }
        private void UpdateUserInitiatives(string[] selectedInitiatives, User userToUpdate)
        {
            if (selectedInitiatives == null)
            {
                if (userToUpdate.Initiatives != null || userToUpdate.Initiatives.Count() > 0)
                    userToUpdate.Initiatives.Clear();
                return;
            }

            var selectedInitiativesHS = new HashSet<string>(selectedInitiatives);
            var userInitiatives = new HashSet<int>(userToUpdate.Initiatives.Select(e => e.ID));

            foreach (var initiative in db.Initiatives)
            {
                if (selectedInitiativesHS.Contains(initiative.ID.ToString()))
                {
                    if (!userInitiatives.Contains(initiative.ID))
                    {
                        userToUpdate.Initiatives.Add(initiative);
                    }
                }
                else
                {
                    if (userInitiatives.Contains(initiative.ID))
                    {
                        userToUpdate.Initiatives.Remove(initiative);
                    }
                }
            }
        }
    }
}
