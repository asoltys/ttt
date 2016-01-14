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
            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventViewModel = new EventViewModel();
            eventViewModel.Event = db.Events.Find(id);
            eventViewModel.Edit = eventViewModel.GetLatestEdit();



            if (eventViewModel.Event == null)
            {
                return HttpNotFound();
            }
            return View(eventViewModel);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin,OPI,Editor")]
        public ActionResult Create(int? id)
        {
            var currentUser = Utils.GetCurrentUser();
            var viewModel = new EventViewModel();
            viewModel.Branches= db.Branches.ToList<Branch>();
            viewModel.Regions = db.Regions.ToList<Region>();

            if (id != null)
            {
                viewModel.InitiativeSelect = new SelectList(db.Initiatives.ToList<Initiative>(), "id", "NameE", id);
            }
            else
            {
                viewModel.InitiativeSelect = new SelectList(db.Initiatives.ToList<Initiative>(), "id", "NameE");
            }
            return View(viewModel);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventViewModel eventViewModel,
            string[] selectedBranches,
            string[] selectedRegions)
        {
            var eventToCreate = eventViewModel.Event;

            if (ModelState.IsValid)
            {
                try
                {
                    eventToCreate.Branches = new List<Branch>();
                    eventToCreate.Regions = new List<Region>();
                    eventToCreate.Edits = new List<Edit>();

                    var selectedBranchesHS = new HashSet<string>(selectedBranches);
                    foreach (var branch in db.Branches)
                    {
                        if (selectedBranches.Contains(branch.ID.ToString()))
                        {
                            eventToCreate.Branches.Add(branch);
                        }
                    }

                    var selectedRegionsHS = new HashSet<string>(selectedRegions);
                    foreach (var region in db.Regions)
                    {
                        if (selectedRegions.Contains(region.ID.ToString()))
                        {
                            eventToCreate.Regions.Add(region);
                        }
                    }

                    var currentUser = Utils.GetCurrentUser();
                    eventViewModel.Edit.Editor = db.Users.Find(currentUser.Id);
                    eventViewModel.Edit.Date = DateTime.Now;

                    eventToCreate.Edits.Add(eventViewModel.Edit);

                    db.Events.Add(eventToCreate);
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

            eventViewModel.Regions = new List<Region>();
            eventViewModel.Branches = new List<Branch>();

            PopulateEventRegionsData(eventViewModel.Event);
            PopulateEventBranchesData(eventViewModel.Event);
            return View(eventViewModel);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Event @event = db.Events.Find(id);
            var eventViewModel = new EventViewModel();
            eventViewModel.Event = db.Events
                .Where(e => e.ID == id)
                .Single();

            eventViewModel.Edit = eventViewModel.GetLatestEdit();
            eventViewModel.InitiativeSelect = new SelectList(db.Initiatives.ToList<Initiative>(), "id", "NameE");

            PopulateEventRegionsData(eventViewModel.Event);
            PopulateEventBranchesData(eventViewModel.Event);

            if (eventViewModel.Event == null)
            {
                return HttpNotFound();
            }

            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", eventViewModel.Event.InitiativeID);
            return View(eventViewModel);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventViewModel eventViewModel, string[] selectedRegions, string[] selectedBranches)
        {

            if (eventViewModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventToUpdate = db.Events
               .Where(i => i.ID == eventViewModel.Event.ID)
               .Single();

            if (ModelState.IsValid)
            {
                try
                {
                    eventToUpdate.InitiativeID = eventViewModel.Event.InitiativeID;
                    eventToUpdate.Date = eventViewModel.Event.Date;
                    eventToUpdate.Type = eventViewModel.Event.Type;
                    eventToUpdate.Status = eventViewModel.Event.Status;

                    UpdateEventRegions(selectedRegions, eventToUpdate);
                    UpdateEventBranches(selectedBranches, eventToUpdate);


                    var currentUser = Utils.GetCurrentUser();
                    eventViewModel.Edit.Editor = db.Users.Find(currentUser.Id);
                    eventViewModel.Edit.Date = DateTime.Now;

                    if (eventViewModel.Event.Status == Status.Approved)
                    {
                        eventToUpdate.PublishedEdit.Published = false;
                        eventViewModel.Edit.Published = true;
                    }

                    eventToUpdate.Edits.Add(eventViewModel.Edit);
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
                    //TextE = e.TextE,
                    //TextF = e.TextF,
                    //HoverE = e.HoverE,
                    //HoverF = e.HoverF,
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

        private void PopulateEventRegionsData(Event @event)
        {
            var allRegions = db.Regions;
            var eventRegions = new HashSet<int>(@event.Regions.Select(r => r.ID));
            var viewModel = new List<RegionsData>();

            foreach (var region in allRegions)
            {
                viewModel.Add(new RegionsData
                {
                    ID = region.ID,
                    NameE = region.NameE,
                    NameF = region.NameF,
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
                    ID = branch.ID,
                    NameE = branch.NameE,
                    NameF = branch.NameF,
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
