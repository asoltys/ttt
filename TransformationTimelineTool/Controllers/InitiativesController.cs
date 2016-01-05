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
using System.DirectoryServices;

namespace TransformationTimelineTool.Controllers
{
    public class InitiativesController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        // GET: Initiatives
        public ActionResult Index(int? id)
        {
            var viewModel = new InitiativeIndexData();

            viewModel.Initiatives = db.Initiatives.
                Include(i => i.Events).
                Include(i => i.Impacts);
            

            if (id != null)
            {
                ViewBag.InitiativeID = id.Value;
                viewModel.Events = viewModel.Initiatives.Where(
                    i => i.ID == id.Value).Single().Events;
                viewModel.Impacts = viewModel.Initiatives.Where(
                    i => i.ID == id.Value).Single().Impacts;
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
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Initiatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NameE,NameF,DescriptionE,DescriptionF,StartDate,EndDate")] Initiative initiative)
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ID,NameE,NameF,DescriptionE,DescriptionF,StartDate,EndDate")] Initiative initiative)
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Initiative initiative = db.Initiatives.Find(id);
            db.Initiatives.Remove(initiative);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Data()
        {
            List<Initiative> initiatives = db.Initiatives.
                Include(e => e.Events).
                Include(i => i.Impacts).ToList();

            return Json(
                initiatives.Select( i => new
                {
                    ID = i.ID,
                    NameE = i.NameE,
                    NameF = i.NameF,
                    DescriptionE = i.DescriptionE,
                    DescriptionF = i.DescriptionF,
                    StartDate = i.StartDate.ToShortDateString(),
                    EndDate = i.EndDate.ToShortDateString(),
                    Events = i.Events.Select( e => new
                    {
                        ID = e.ID,
                        Type = e.Type.ToString(),
                        Date = e.Date.ToShortDateString(),
                        Branches = e.Branches.Select(b => b.ID),
                        Regions = e.Regions.Select(r => r.ID),
                        TextE = e.TextE,
                        HoverE = e.HoverE,
                        TextF = e.TextF,
                        HoverF = e.HoverF,
                        Show = e.Show
                    }),
                    Impacts = i.Impacts.Select( imp => new
                    {
                        Level = imp.Level,
                        Branches = imp.Branches.Select(b => b.ID),
                        Regions = imp.Regions.Select(r => r.ID)

                    })
                }) , JsonRequestBehavior.AllowGet);
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
