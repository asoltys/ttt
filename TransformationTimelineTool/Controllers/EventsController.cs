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
    public class EventsController : Controller
    {
        private TimelineContext db = new TimelineContext();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(e => e.Branch).Include(e => e.Initiative).Include(e => e.Regions);
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
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameE");
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE");
            ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameE");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InitiativeID,BranchID,RegionID,Type,Date,TextE,TextF,HoverE,HoverF")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameShort", @event.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", @event.InitiativeID);
            //ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameShort", @event.RegionID);
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
                .Include(e => e.Regions)
                .Where(e => e.ID == id)
                .Single();
            PopulateEventRegionsData(@event);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameE", @event.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", @event.InitiativeID);
            //ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameE", @event.RegionID);
            return View(@event);
        }

        private void PopulateEventRegionsData(Event @event)
        {
            var allRegions = db.Regions;
            var eventRegions = new HashSet<int>(@event.Regions.Select(r => r.ID));
            var viewModel = new List<EventRegionsData>();

            foreach(var region in allRegions)
            {
                viewModel.Add(new EventRegionsData
                {
                    RegionID = region.ID,
                    RegionNameE = region.NameE,
                    RegionNameF = region.NameF,
                    Flag = eventRegions.Contains(region.ID)
                });
            }
            ViewBag.Regions = viewModel;
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedRegions)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventToUpdate = db.Events
               .Include(e => e.Regions)
               .Where(i => i.ID == id)
               .Single();

            if (TryUpdateModel(eventToUpdate, "",
               new string[] { "InitiativeID,BranchID,Type,Date,TextE,TextF,HoverE,HoverF" }))
            {
                try
                {
                    UpdateEventRegions(selectedRegions, eventToUpdate);

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
