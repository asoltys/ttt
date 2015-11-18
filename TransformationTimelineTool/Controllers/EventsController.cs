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

namespace TransformationTimelineTool.Controllers
{
    public class EventsController : Controller
    {
        private TimelineContext db = new TimelineContext();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(e => e.Branch).Include(e => e.Initiative).Include(e => e.Region);
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
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameShort");
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE");
            ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameShort");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InitiativeID,BranchID,RegionID,Date,TextE,TextF,HoverE,HoverF")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameShort", @event.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", @event.InitiativeID);
            ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameShort", @event.RegionID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameShort", @event.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", @event.InitiativeID);
            ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameShort", @event.RegionID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,InitiativeID,BranchID,RegionID,Date,TextE,TextF,HoverE,HoverF")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameShort", @event.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", @event.InitiativeID);
            ViewBag.RegionID = new SelectList(db.Regions, "ID", "NameShort", @event.RegionID);
            return View(@event);
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
