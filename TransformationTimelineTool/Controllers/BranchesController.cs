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
    [RoutePrefix("Directions-Generales-Branches")]
    [Route("{action=index}")]
    public class BranchesController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        // GET: Branches
        [Authorize(Roles = "Admin,Approver,Editor")]
        public ActionResult Index()
        {
            return View(db.Branches.ToList());
        }

        // GET: Branches/Details/5
        [Authorize(Roles = "Admin,Approver,Editor")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        [Authorize(Roles = "Admin")]
        [Route("Creer-Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Creer-Create")]
        public ActionResult Create([Bind(Include = "ID,NameShort,NameE,NameF")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit([Bind(Include = "ID,NameShort,NameE,NameF")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }
            return View(branch);
        }
        [AllowAnonymous]
        public ActionResult Data()
        {
            var jsonBranches = new List<object>();
            
            List<Branch> branches= db.Branches.ToList();

            foreach (var branch in branches)
            {
                jsonBranches.Add(new
                {
                    ID = branch.ID,
                    NameShort = branch.NameShort,
                    NameE = branch.NameE,
                    NameF = branch.NameF
                });
            }

            return Json(jsonBranches, JsonRequestBehavior.AllowGet);
        }

        // GET: Branches/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            db.Branches.Remove(branch);
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
