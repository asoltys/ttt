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
            var events = db.Events.Include(e => e.Branches).Include(e => e.Initiative).Include(e => e.Regions);
            return View(events.ToList());
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
        public ActionResult Create()
        {
            ViewBag.Branches = db.Branches.OrderBy(b => b.NameE).ToList<Branch>();
            ViewBag.Regions = db.Regions.OrderBy(r => r.NameE).ToList<Region>();
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InitiativeID,Type,Date,TextE,TextF,HoverE,HoverF")] Event @event, string[] selectedBranches, string[] selectedRegions)
        {
            if (TryUpdateModel(@event, "",
               new string[] { "InitiativeID,Type,Date,TextE,TextF,HoverE,HoverF" }))
            {
                try
                {
                    //UpdateEventRegions(selectedRegions, @event);
                    //UpdateEventBranches(selectedBranches, @event);
                    @event.Branches = new List<Branch>();
                    @event.Regions= new List<Region>();
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
                    db.Events.Add(@event);
                    db.SaveChanges();

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

            foreach(var region in allBranches)
            {
                viewModel.Add(new BranchesData
                {
                    BranchID = region.ID,
                    BranchNameE = region.NameE,
                    BranchNameF = region.NameF,
                    Flag = eventBranches.Contains(region.ID)
                });
            }
            ViewBag.Branches = viewModel;
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event @event, string[] selectedRegions, string[] selectedBranches)
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

        private void UpdateEventBranches (string[] selectedBranches, Event eventToUpdate)
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

            foreach(var region in db.Regions)
            {
                if (selectedRegionHS.Contains(region.ID.ToString())){
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
    }
}
