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

namespace TransformationTimelineTool.Controllers
{
    public class UsersController : Controller
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
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
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

        // GET: Users/Create
        public ActionResult Create()
        {
            var userViewModel = new UserViewModel();

            userViewModel.Branches = db.Branches.ToList<Branch>();
            userViewModel.Regions = db.Regions.ToList<Region>();
            userViewModel.Roles = db.Roles.ToList<IdentityRole>();
            userViewModel.ApproverSelect = new SelectList(Utils.GetOPIs(), "Id", "UserName");

            return View(userViewModel);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel userViewModel,
            string[] selectedBranches,
            string[] selectedRegions,
            string[] selectedRoles)
        {
            string username = Utils.GetUsernameFromEmail(userViewModel.User.Email);
            var userToAdd = new User();

            userToAdd.Branches = new List<Branch>();
            userToAdd.Regions = new List<Region>();

            var selectedBranchesHS = new HashSet<string>(selectedBranches);
            foreach (var branch in db.Branches)
            {
                if (selectedBranches.Contains(branch.ID.ToString()))
                {
                    userToAdd.Branches.Add(branch);
                }
            }

            var selectedRegionsHS = new HashSet<string>(selectedRegions);
            foreach (var region in db.Regions)
            {
                if (selectedRegions.Contains(region.ID.ToString()))
                {
                    userToAdd.Regions.Add(region);
                }
            }

            if (username != "")
            {
                userToAdd.UserName = username;
                userToAdd.Email = userViewModel.User.Email;
                userToAdd.ApproverID = userViewModel.User.ApproverID;

                var myResult = userManager.Create(userToAdd, "password");

                if (myResult.Succeeded)
                {
                    var result = userManager.AddToRoles(userToAdd.Id, selectedRoles);
                }
                
                return RedirectToAction("Index");
            }

            PopulateUserRegionsData(userToAdd);
            PopulateUserBranchesData(userToAdd);
            PopulateUserRolesData(userToAdd);

            return View(userViewModel);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userViewModel = new UserViewModel();

            User user = db.Users.Find(id);

            userViewModel.User = user;
            userViewModel.PopulatedBranches = PopulateUserBranchesData(user);
            userViewModel.PopulatedRegions = PopulateUserRegionsData(user);
            userViewModel.PopulatedRoles = PopulateUserRolesData(user);
            userViewModel.ApproverSelect = new SelectList(Utils.GetOPIs(), "Id", "UserName");

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
        public ActionResult Edit(UserViewModel userViewModel,
            string[] selectedRegions,
            string[] selectedBranches,
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

                UpdateUserRegions(selectedRegions, userToUpdate);
                UpdateUserBranches(selectedBranches, userToUpdate);
                UpdateUserRoles(selectedRoles, userToUpdate);

                var myResult = userManager.Update(userToUpdate);


                return RedirectToAction("Index");
            }
            return View(userViewModel);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<RegionsData> PopulateUserRegionsData(User user)
        {
            var allRegions = db.Regions;
            var userRegions = new HashSet<int>(user.Regions.Select(r => r.ID));
            var viewModel = new List<RegionsData>();

            foreach (var region in allRegions)
            {
                viewModel.Add(new RegionsData
                {
                    ID = region.ID,
                    NameE = region.NameE,
                    NameF = region.NameF,
                    Flag = userRegions.Contains(region.ID)
                });
            }

            return viewModel;
        }

        private List<BranchesData> PopulateUserBranchesData(User user)
        {
            var allBranches = db.Branches;
            var userBranches = new HashSet<int>(user.Branches.Select(b => b.ID));
            var viewModel = new List<BranchesData>();

            foreach (var branch in allBranches)
            {
                viewModel.Add(new BranchesData
                {
                    ID = branch.ID,
                    NameE = branch.NameE,
                    NameF = branch.NameF,
                    Flag = userBranches.Contains(branch.ID)
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

        private void UpdateUserBranches(string[] selectedBranches, User userToUpdate)
        {
            if (selectedBranches == null)
            {
                userToUpdate.Branches = new List<Branch>();
                return;
            }

            var selectedBranchesHS = new HashSet<string>(selectedBranches);
            var userBranches = new HashSet<int>(userToUpdate.Branches.Select(e => e.ID));

            foreach (var branch in db.Branches)
            {
                if (selectedBranches.Contains(branch.ID.ToString()))
                {
                    if (!userBranches.Contains(branch.ID))
                    {
                        userToUpdate.Branches.Add(branch);
                    }
                }
                else
                {
                    if (userBranches.Contains(branch.ID))
                    {
                        userToUpdate.Branches.Remove(branch);
                    }
                }
            }
        }

        private void UpdateUserRegions(string[] selectedRegions, User userToUpdate)
        {
            if (selectedRegions == null)
            {
                userToUpdate.Regions = new List<Region>();
                return;
            }

            var selectedRegionHS = new HashSet<string>(selectedRegions);
            var userRegions = new HashSet<int>(userToUpdate.Regions.Select(e => e.ID));

            foreach (var region in db.Regions)
            {
                if (selectedRegionHS.Contains(region.ID.ToString()))
                {
                    if (!userRegions.Contains(region.ID))
                    {
                        userToUpdate.Regions.Add(region);
                    }
                }
                else
                {
                    if (userRegions.Contains(region.ID))
                    {
                        userToUpdate.Regions.Remove(region);
                    }
                }
            }
        }

        private void UpdateUserRoles(string[] selectedRoles, User userToUpdate)
        {
            userManager.RemoveFromRoles(userToUpdate.Id, userManager.GetRoles(userToUpdate.Id).ToArray());
            if (selectedRoles != null)
            {
                userManager.AddToRoles(userToUpdate.Id, selectedRoles);
            }
        }
    }
}
