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
using TransformationTimelineTool.ViewModels;

namespace TransformationTimelineTool.Controllers
{
    public class InitiativesController : Controller
    {
        private TimelineContext db = new TimelineContext();

        // GET: Initiatives
        public ActionResult Index(int? id)
        {
            var viewModel = new InitiativeIndexData();

            viewModel.Initiatives = db.Initiatives.
                Include(i => i.Events);

            if (id != null)
            {
                ViewBag.InitiativeID = id.Value;
                viewModel.Events = viewModel.Initiatives.Where(
                    i => i.ID == id.Value).Single().Events;
            }
            return View(viewModel);
        }

        // GET: Initiatives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Initiative initiative = db.Initiatives.Find(id);
            if (initiative == null)
            {
                return HttpNotFound();
            }
            return View(initiative);
        }

        // GET: Initiatives/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Initiatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NameE,Description,StartDate,EndDate")] Initiative initiative)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Initiatives.Add(initiative);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(initiative);

        }

        // GET: Initiatives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Initiative initiative = db.Initiatives.Find(id);
            if (initiative == null)
            {
                return HttpNotFound();
            }
            return View(initiative);
        }

        // POST: Initiatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameE,Description,StartDate,EndDate")] Initiative initiative)
        {
            if (ModelState.IsValid)
            {
                db.Entry(initiative).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(initiative);
        }

        // GET: Initiatives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Initiative initiative = db.Initiatives.Find(id);
            if (initiative == null)
            {
                return HttpNotFound();
            }
            return View(initiative);
        }

        // POST: Initiatives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Initiative initiative = db.Initiatives.Find(id);
            db.Initiatives.Remove(initiative);
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
