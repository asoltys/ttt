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
    public class ImpactsController : Controller
    {
        private TimelineContext db = new TimelineContext();

        // GET: Impacts
        public ActionResult Index()
        {
            var impacts = db.Impacts.
                Include(i => i.Branches).
                Include(i => i.Regions).
                Include(i => i.Initiative).
                OrderBy(i => i.Initiative.NameE);
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
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameE");
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE");
            return View();
        }

        // POST: Impacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InitiativeID,BranchID,Level")] Impact impact)
        {
            if (ModelState.IsValid)
            {
                db.Impacts.Add(impact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameE", impact.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", impact.InitiativeID);
            return View(impact);
        }

        // GET: Impacts/Edit/5
        public ActionResult Edit(int? id)
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
            //ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameE", impact.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", impact.InitiativeID);
            return View(impact);
        }

        // POST: Impacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,InitiativeID,BranchID,Level")] Impact impact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(impact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.BranchID = new SelectList(db.Branches, "ID", "NameE", impact.BranchID);
            ViewBag.InitiativeID = new SelectList(db.Initiatives, "ID", "NameE", impact.InitiativeID);
            return View(impact);
        }

        // GET: Impacts/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Impact impact = db.Impacts.Find(id);
            db.Impacts.Remove(impact);
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
