﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.ViewModels;

namespace TransformationTimelineTool.Controllers
{
    [Authorize(Roles = "Admin,Approver,Editor")]
    [RoutePrefix("Repercussions-Impacts")]
    [Route("{action=index}")]
    public class ImpactsController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        // GET: Impacts
        public ActionResult Index()
        {
            IOrderedQueryable<Impact> impacts;
            if (Thread.CurrentThread.CurrentCulture.Name == "fr")
            {
                impacts = db.Impacts.
                    Include(i => i.Branches).
                    Include(i => i.Regions).
                    Include(i => i.Initiative).
                    OrderBy(i => i.Initiative.NameE);
            }
            else
            {
                impacts = db.Impacts.
                    Include(i => i.Branches).
                    Include(i => i.Regions).
                    Include(i => i.Initiative).
                    OrderBy(i => i.Initiative.NameE);
            }
            return View(impacts.ToList());
        }

        // GET: Impacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impact impact = db.Impacts.Find(id);
            if (impact == null)
            {
                return HttpNotFound();
            }
            return View(impact);
        }

        // GET: Impacts/Create
        [Authorize(Roles = "Admin")]
        [Route("Creer-Create")]
        public ActionResult Create(int? id)
        {
            
            List<Initiative> initiatives;
            if (Thread.CurrentThread.CurrentCulture.Name == "fr")
            {
                ViewBag.Branches = db.Branches.OrderBy(b => b.NameF).ToList<Branch>();
                ViewBag.Regions = db.Regions.OrderBy(r => r.NameF).ToList<Region>();
                initiatives = db.Initiatives.OrderBy(b => b.NameF).ToList<Initiative>();
            } else
            {
                ViewBag.Branches = db.Branches.OrderBy(b => b.NameE).ToList<Branch>();
                ViewBag.Regions = db.Regions.OrderBy(r => r.NameE).ToList<Region>();
                initiatives = db.Initiatives.OrderBy(b => b.NameE).ToList<Initiative>();
            }
                
            if (id != null)
            {
                ViewBag.InitiativeID = new SelectList(initiatives, "ID", "Name", id);
            }
            else
            {
                ViewBag.InitiativeID = new SelectList(initiatives, "ID", "Name");
            }

            List<SelectListItem> LevelItems = new List<SelectListItem>();
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.None, Value = "0", Selected = true });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.Low, Value = "1" });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.Medium, Value = "2" });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.High, Value = "3" });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.BP2020, Value = "4" });
            ViewBag.Level = LevelItems;
            return View();
        }

        // POST: Impacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Creer-Create")]
        public ActionResult Create([Bind(Include = "ID,InitiativeID,BranchID,Justification,Level")] Impact impact, string[] selectedBranches, string[] selectedRegions)
        {
            if (ModelState.IsValid)
            {
                impact.Branches = new List<Branch>();
                impact.Regions = new List<Region>();
                
                foreach (var branch in db.Branches)
                {
                    if (selectedBranches.Contains(branch.ID.ToString()))
                    {
                        impact.Branches.Add(branch);
                    }
                }
                
                foreach (var region in db.Regions)
                {
                    if (selectedRegions.Contains(region.ID.ToString()))
                    {
                        impact.Regions.Add(region);
                    }
                }

                impact.Edited = true;
                db.Impacts.Add(impact);
                db.SaveChanges();
                
                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }
            return View(impact);
        }

        // GET: Impacts/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Impact impact = db.Impacts
                .Include(i => i.Regions)
                .Include(i => i.Branches)
                .Where(i => i.ID == id)
                .Single();

            PopulateEventRegionsData(impact);
            PopulateEventBranchesData(impact);

            if (impact == null)
            {
                return HttpNotFound();
            }
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "Name", impact.InitiativeID);
            List<SelectListItem> LevelItems = new List<SelectListItem>();
            Helpers.Utils.log(impact.Level.ToString());
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.None, Value = "0",
                Selected = impact.Level.ToString() == "None" ? true : false
            });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.Low, Value = "1",
                Selected = impact.Level.ToString() == "Low" ? true : false
            });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.Medium, Value = "2",
                Selected = impact.Level.ToString() == "Medium" ? true : false
            });
            LevelItems.Add(new SelectListItem { Text = Resources.Resources.High, Value = "3",
                Selected = impact.Level.ToString() == "High" ? true : false
            });
            LevelItems.Add(new SelectListItem
            {
                Text = Resources.Resources.BP2020,
                Value = "4",
                Selected = impact.Level.ToString() == "BP2020" ? true : false
            });
            ViewBag.Level = LevelItems;
            return View(impact);
        }

        private void PopulateEventRegionsData(Impact impact)
        {
            string Culture = Thread.CurrentThread.CurrentCulture.Name;
            var allRegions = Culture == "fr" ? db.Regions.OrderBy(r => r.NameF) : db.Regions.OrderBy(r => r.NameE);
            var impactRegions = new HashSet<int>(impact.Regions.Select(r => r.ID));
            var viewModel = new List<RegionsData>();

            foreach (var region in allRegions)
            {
                viewModel.Add(new RegionsData
                {
                    ID = region.ID,
                    NameE = region.NameE,
                    NameF = region.NameF,
                    Flag = impactRegions.Contains(region.ID)
                });
            }
            ViewBag.Regions = viewModel;
        }

        private void PopulateEventBranchesData(Impact impact)
        {
            string Culture = Thread.CurrentThread.CurrentCulture.Name;
            var allBranches = Culture == "fr" ? db.Branches.OrderBy(b => b.NameF) : db.Branches.OrderBy(b => b.NameE);
            var impactBranches = new HashSet<int>(impact.Branches.Select(b => b.ID));
            var viewModel = new List<BranchesData>();

            foreach (var branch in allBranches)
            {
                viewModel.Add(new BranchesData
                {
                    ID = branch.ID,
                    NameE = branch.NameE,
                    NameF = branch.NameF,
                    Flag = impactBranches.Contains(branch.ID)
                });
            }
            ViewBag.Branches = viewModel;
        }

        // POST: Impacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit(Impact impact, string[] selectedRegions, string[] selectedBranches)
        {

            if (impact == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var impactToUpdate = db.Impacts
               .Where(i => i.ID == impact.ID)
               .Single();

            if (ModelState.IsValid)
            {
                try
                {
                    impactToUpdate.InitiativeID = impact.InitiativeID;
                    impactToUpdate.Level = impact.Level;
                    impactToUpdate.Justification = impact.Justification;
                    impactToUpdate.Edited = true;
                    UpdateImpactRegions(selectedRegions, impactToUpdate);
                    UpdateImpactBranches(selectedBranches, impactToUpdate);

                    db.SaveChanges();
                    ManageCache();
                    return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateEventRegionsData(impactToUpdate);
            return View(impactToUpdate);
        }


        private void UpdateImpactBranches(string[] selectedBranches, Impact impactToUpdate)
        {
            if (selectedBranches == null)
            {
                impactToUpdate.Branches = new List<Branch>();
                return;
            }

            var selectedBranchesHS = new HashSet<string>(selectedBranches);
            var impactBranches = new HashSet<int>(impactToUpdate.Branches.Select(e => e.ID));

            foreach (var branch in db.Branches)
            {
                if (selectedBranches.Contains(branch.ID.ToString()))
                {
                    if (!impactBranches.Contains(branch.ID))
                    {
                        impactToUpdate.Branches.Add(branch);
                    }
                }
                else
                {
                    if (impactBranches.Contains(branch.ID))
                    {
                        impactToUpdate.Branches.Remove(branch);
                    }
                }
            }
        }

        private void UpdateImpactRegions(string[] selectedRegions, Impact impactToUpdate)
        {
            if (selectedRegions == null)
            {
                impactToUpdate.Regions = new List<Region>();
                return;
            }

            var selectedRegionHS = new HashSet<string>(selectedRegions);
            var impactRegions = new HashSet<int>(impactToUpdate.Regions.Select(e => e.ID));

            foreach (var region in db.Regions)
            {
                if (selectedRegionHS.Contains(region.ID.ToString()))
                {
                    if (!impactRegions.Contains(region.ID))
                    {
                        impactToUpdate.Regions.Add(region);
                    }
                }
                else
                {
                    if (impactRegions.Contains(region.ID))
                    {
                        impactToUpdate.Regions.Remove(region);
                    }
                }
            }
        }

        // GET: Impacts/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impact impact = db.Impacts.Find(id);
            if (impact == null)
            {
                return HttpNotFound();
            }
            return View(impact);
        }

        // POST: Impacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Impact impact = db.Impacts.Find(id);
            db.Impacts.Remove(impact);
            db.SaveChanges();
            return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
