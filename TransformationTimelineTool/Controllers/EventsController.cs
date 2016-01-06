using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.ViewModels;

namespace TransformationTimelineTool.Controllers
{
    public class EventsController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        // GET: Events
        public ActionResult Index()
        {
            var currentUser = Utils.GetCurrentUser();
            return View(currentUser.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin,OPI")]
        public ActionResult Create(int? id)
        {
            var currentUser = Utils.GetCurrentUser();
            ViewBag.Branches = currentUser.Branches.ToList<Branch>();
            ViewBag.Regions = currentUser.Regions.ToList<Region>();

            if (id != null)
            {
                ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", id);
            }
            else
            {
                ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE");
            }
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InitiativeID,Type,Date,TextE,TextF,HoverE,HoverF")] Event @event,
            string[] selectedBranches,
            string[] selectedRegions)
        {
            if (TryUpdateModel(@event, "",
               new string[] { "InitiativeID,Type,Date,TextE,TextF,HoverE,HoverF" }))
            {
                try
                {
                    @event.Branches = new List<Branch>();
                    @event.Regions = new List<Region>();
                    var selectedBranchesHS = new HashSet<string>(selectedBranches);
                    foreach (var branch in db.Branches)
                    {
                        if (selectedBranches.Contains(branch.ID.ToString()))
                        {
                            @event.Branches.Add(branch);
                        }
                    }

                    var selectedRegionsHS = new HashSet<string>(selectedRegions);
                    foreach (var region in db.Regions)
                    {
                        if (selectedRegions.Contains(region.ID.ToString()))
                        {
                            @event.Regions.Add(region);
                        }
                    }

                    CreateEdit(@event, Status.Created);
                    db.SaveChanges();

                    //@event.Edit = edit;;

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateEventRegionsData(@event);
            PopulateEventBranchesData(@event);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Event @event = db.Events.Find(id);
            Event @event = db.Events
                .Where(e => e.ID == id)
                .Single();
            PopulateEventRegionsData(@event);
            PopulateEventBranchesData(@event);

            if (@event == null)
            {
                return HttpNotFound();
            }

            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", @event.InitiativeID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event @event, string[] selectedRegions, string[] selectedBranches, int status)
        {

            if (@event == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventToUpdate = db.Events
               .Where(i => i.ID == @event.ID)
               .Single();

            if (ModelState.IsValid)
            {
                try
                {
                    eventToUpdate.InitiativeID = @event.InitiativeID;
                    eventToUpdate.TextE = @event.TextE;
                    eventToUpdate.TextF = @event.TextF;
                    eventToUpdate.HoverE = @event.HoverE;
                    eventToUpdate.HoverF = @event.HoverF;
                    eventToUpdate.Date = @event.Date;
                    eventToUpdate.Type = @event.Type;

                    UpdateEventRegions(selectedRegions, eventToUpdate);
                    UpdateEventBranches(selectedBranches, eventToUpdate);

                    CreateEdit(eventToUpdate, (Status)status);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateEventRegionsData(eventToUpdate);
            return View(eventToUpdate);
        }

        public ActionResult Data(string branch, string region)
        {
            var regions = db.Regions.Where(r => r.NameShort == region).SelectMany(r => r.Events).ToList();
            var branches = db.Branches.Where(r => r.NameShort == branch).SelectMany(r => r.Events).ToList();
            var events = regions.Intersect(branches);

            return Json(
                events.Select(e => new
                {
                    ID = e.ID,
                    Date = e.Date.ToShortDateString(),
                    TextE = e.TextE,
                    TextF = e.TextF,
                    HoverE = e.HoverE,
                    HoverF = e.HoverF,
                    InitiativeID = e.InitiativeID
                }), JsonRequestBehavior.AllowGet);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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

        private void CreateEdit(Event @event, Status status)
        {
            var currentUser = Utils.GetCurrentUser();

            Edit edit = new Edit
            {
                Editor = db.Users.Find(currentUser.Id),
                Date = DateTime.Now,
                Event = @event,
                Status = status
            };

            db.Edits.Add(edit);
        }

        private void PopulateEventRegionsData(Event @event)
        {
            var allRegions = db.Regions;
            var eventRegions = new HashSet<int>(@event.Regions.Select(r => r.ID));
            var viewModel = new List<RegionsData>();

            foreach (var region in allRegions)
            {
                viewModel.Add(new RegionsData
                {
                    RegionID = region.ID,
                    RegionNameE = region.NameE,
                    RegionNameF = region.NameF,
                    Flag = eventRegions.Contains(region.ID)
                });
            }
            ViewBag.Regions = viewModel;
        }

        private void PopulateEventBranchesData(Event @event)
        {
            var allBranches = db.Branches;
            var eventBranches = new HashSet<int>(@event.Branches.Select(b => b.ID));
            var viewModel = new List<BranchesData>();

            foreach (var branch in allBranches)
            {
                viewModel.Add(new BranchesData
                {
                    BranchID = branch.ID,
                    BranchNameE = branch.NameE,
                    BranchNameF = branch.NameF,
                    Flag = eventBranches.Contains(branch.ID)
                });
            }
            ViewBag.Branches = viewModel;
        }

        private void UpdateEventBranches(string[] selectedBranches, Event eventToUpdate)
        {
            if (selectedBranches == null)
            {
                eventToUpdate.Branches = new List<Branch>();
                return;
            }

            var selectedBranchesHS = new HashSet<string>(selectedBranches);
            var eventBranches = new HashSet<int>(eventToUpdate.Branches.Select(e => e.ID));

            foreach (var branch in db.Branches)
            {
                if (selectedBranches.Contains(branch.ID.ToString()))
                {
                    if (!eventBranches.Contains(branch.ID))
                    {
                        eventToUpdate.Branches.Add(branch);
                    }
                }
                else
                {
                    if (eventBranches.Contains(branch.ID))
                    {
                        eventToUpdate.Branches.Remove(branch);
                    }
                }
            }
        }

        private void UpdateEventRegions(string[] selectedRegions, Event eventToUpdate)
        {
            if (selectedRegions == null)
            {
                eventToUpdate.Regions = new List<Region>();
                return;
            }

            var selectedRegionHS = new HashSet<string>(selectedRegions);
            var eventRegions = new HashSet<int>(eventToUpdate.Regions.Select(e => e.ID));

            foreach (var region in db.Regions)
            {
                if (selectedRegionHS.Contains(region.ID.ToString()))
                {
                    if (!eventRegions.Contains(region.ID))
                    {
                        eventToUpdate.Regions.Add(region);
                    }
                }
                else
                {
                    if (eventRegions.Contains(region.ID))
                    {
                        eventToUpdate.Regions.Remove(region);
                    }
                }
            }
        }
    }
}
