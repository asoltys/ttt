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
        public ActionResult Data()
        {
            var jsonInitiatives = new List<object>();
            
            List<Initiative> initiatives = db.Initiatives.ToList();

            foreach (var init in initiatives)
            {
                var jsonEvents = new List<object>();
                var jsonImpacts = new List<object>();

                foreach (var e in init.Events)
                {
                    jsonEvents.Add(new
                    {
                        ID = e.ID,
                        Type = e.Type.ToString(),
                        Date = e.Date.ToShortDateString(),
                        Branches = e.Branches.Select(b => b.ID),
                        Regions = e.Regions.Select(r => r.ID),
                        TextE = e.TextE,
                        HoverE = e.HoverE,
                        TextF = e.TextF,
                        HoverF = e.HoverF
                    });
                }

                foreach (var impact in init.Impacts)
                {
                    jsonImpacts.Add(new
                    {
                        //Branch = impact.Branch.NameShort,
                        Level = impact.Level,
                        Branches = impact.Branches.Select(b => b.ID),
                        Regions = impact.Regions.Select(r => r.ID)
                    });
                }

                jsonInitiatives.Add(new
                {
                    ID = init.ID,
                    NameE = init.NameE,
                    NameF = init.NameF,
                    DescriptionE = init.DescriptionE,
                    DescriptionF = init.DescriptionF,
                    StartDate = init.StartDate.ToShortDateString(),
                    EndDate = init.EndDate.ToShortDateString(),
                    Impacts = jsonImpacts,
                    Events = jsonEvents
                });
            }

            return Json(jsonInitiatives, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Authenticate(string login, string password)
        {
            DirectoryEntry root = new DirectoryEntry(
                "LDAP://adldap.ncr.pwgsc.gc.ca/dc=ad,dc=pwgsc-tpsgc,dc=gc,dc=ca",
                login,
                password);

            DirectorySearcher searcher = new DirectorySearcher(
                root,
                "(mailNickname=" + login + ")");

            bool success = false;
            SearchResult person;

            try
            {
                person = searcher.FindOne();
                success = true;
            }
            catch
            {
                success = false;
            }

            //jsonData["Verified"] = true;
            return Json(new { Verified = success }, JsonRequestBehavior.AllowGet);

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
