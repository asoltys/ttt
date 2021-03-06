﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.Controllers
{
    [RoutePrefix("Regions")]
    [Route("{action=index}")]
    public class RegionsController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        // GET: Regions
        [Authorize(Roles = "Admin,Approver,Editor")]
        public ActionResult Index()
        {
            return View(db.Regions.ToList());
        }

        // GET: Regions/Details/5
        [Authorize(Roles = "Admin,Approver,Editor")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // GET: Regions/Create
        [Authorize(Roles = "Admin")]
        [Route("Creer-Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Creer-Create")]
        public ActionResult Create([Bind(Include = "ID,NameShort,NameE,NameF")] Region region)
        {
            if (ModelState.IsValid)
            {
                db.Regions.Add(region);
                db.SaveChanges();
                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }

            return View(region);
        }

        // GET: Regions/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit([Bind(Include = "ID,NameShort,NameE,NameF")] Region region)
        {
            if (ModelState.IsValid)
            {
                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }
            return View(region);
        }
        [AllowAnonymous]
        public ActionResult Data()
        {
            var jsonRegions = new List<object>();

            List<Region> regions = db.Regions.ToList();

            foreach (var region in regions)
            {
                jsonRegions.Add(new
                {
                    ID = region.ID,
                    NameShort = region.NameShort,
                    NameE = region.NameE,
                    NameF = region.NameF
                });
            }

            return Json(jsonRegions, JsonRequestBehavior.AllowGet);
        }

        // GET: Regions/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Region region = db.Regions.Find(id);
            db.Regions.Remove(region);
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
