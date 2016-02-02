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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TransformationTimelineTool.Helpers;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace TransformationTimelineTool.Controllers
{
    [Authorize(Roles = "Admin,Approver,Editor")]
    [RoutePrefix("Initiatives")]
    [Route("{action=index}")]
    public class InitiativesController : BaseController
    {
        private TimelineContext db = new TimelineContext();
        private UserManager<User> userManager;

        public InitiativesController()
        { 
            userManager = new UserManager<User>(new UserStore<User>(db));
        }
        // GET: Initiatives
        public ActionResult Index(int? id)
        {
            var viewModel = new InitiativeIndexData();

            var currentUser = Utils.GetCurrentUser();

            viewModel.Initiatives = db.Initiatives.
                Include(i => i.Events).
                Include(i => i.Impacts);
            

            if (id != null)
            {
                ViewBag.InitiativeID = id.Value;
                viewModel.Events = db.Events.Where(e => e.CreatorID == currentUser.Id);
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
        [Route("Creer-Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Initiatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Creer-Create")]
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
        [Route("Modifier-Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var initiativeViewModel = new InitiativeViewModel(id);

            if (initiativeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(initiativeViewModel);
        }

        // POST: Initiatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Modifier-Edit")]
        public ActionResult Edit(InitiativeViewModel initiativeViewModel)
        {
            if (initiativeViewModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var initiativeToUpdate = db.Initiatives
               .Where(i => i.ID == initiativeViewModel.ID)
               .Single();

            if (ModelState.IsValid)
            {
                try
                {
                    initiativeViewModel.BindToModel(initiativeToUpdate);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
              
            }
            return View(initiativeViewModel);
        }

        // GET: Initiatives/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Supprimer-Delete")]
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
        [Route("Supprimer-Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Initiative initiative = db.Initiatives.Find(id);
            db.Initiatives.Remove(initiative);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [OutputCache(Duration = 3600,
            Location = System.Web.UI.OutputCacheLocation.Server,
            VaryByParam = "*")]
        public async Task<ActionResult> Data()
        {
            List<Initiative> initiatives = await db.Initiatives.
                Include(e => e.Events).
                Include(i => i.Impacts).ToListAsync();

            return Json(
                initiatives.Select(i => new
                {
                    ID = i.ID,
                    NameE = i.NameE,
                    NameF = i.NameF,
                    DescriptionE = i.DescriptionE,
                    DescriptionF = i.DescriptionF,
                    StartDate = i.StartDate.ToShortDateString(),
                    EndDate = i.EndDate.ToShortDateString(),
                    Events = i.Events.Select(e => new EventJSON(e.ID)),
                    Impacts = i.Impacts.Select(imp => new
                    {
                        Level = imp.Level,
                        Branches = imp.Branches.Select(b => b.ID),
                        Regions = imp.Regions.Select(r => r.ID)

                    })
                }), JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [OutputCache(Duration = 3600,
            Location = System.Web.UI.OutputCacheLocation.Server,
            SqlDependency = "CommandNotification")]
        public async Task<ActionResult> DataCache()
        {
            List<Initiative> initiatives = await db.Initiatives.
                Include(e => e.Events).
                Include(i => i.Impacts).ToListAsync();

            return Json(
                initiatives.Select(i => new
                {
                    ID = i.ID,
                    NameE = i.NameE,
                    NameF = i.NameF,
                    DescriptionE = i.DescriptionE,
                    DescriptionF = i.DescriptionF,
                    StartDate = i.StartDate.ToShortDateString(),
                    EndDate = i.EndDate.ToShortDateString(),
                    Events = i.Events.Select(e => new EventJSON(e.ID)),
                    Impacts = i.Impacts.Select(imp => new
                    {
                        Level = imp.Level,
                        Branches = imp.Branches.Select(b => b.ID),
                        Regions = imp.Regions.Select(r => r.ID)

                    })
                }), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<ActionResult> DataUni()
        {
            var viewModel = new List<object>();
            var jsonInitiatives = new List<object>();

            List<Initiative> initiatives = await db.Initiatives.ToListAsync();

            if (Thread.CurrentThread.CurrentCulture.Name == "fr")
            {
                DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("en-fr").DateTimeFormat;
                dtfi.ShortDatePattern = @"M/d/yyyy";

                foreach (var init in initiatives)
                {
                    var jsonEvents = new List<object>();
                    var jsonImpacts = new List<object>();

                    foreach (var e in init.Events)
                    {
                        jsonEvents.Add(new
                        {
                            ID = e.ID,
                            Type = e.PublishedEdit.Type.ToString(),
                            Date = e.PublishedEdit.DisplayDate.ToString("d",dtfi),
                            Branches = e.Branches.Select(b => b.ID),
                            Regions = e.Regions.Select(r => r.ID),
                            Text = e.PublishedEdit.TextF,
                            Hover = e.PublishedEdit.HoverF,
                            Show = e.Show
                        });
                    }

                    foreach (var impact in init.Impacts)
                    {
                        jsonImpacts.Add(new
                        {
                            Level = impact.Level,
                            Branches = impact.Branches.Select(b => b.ID),
                            Regions = impact.Regions.Select(r => r.ID)
                        });
                    }

                    jsonInitiatives.Add(new
                    {
                        ID = init.ID,
                        Name = init.NameF,
                        Description = init.DescriptionF,
                        StartDate = init.StartDate.ToString("d", dtfi),
                        EndDate = init.EndDate.ToString("d",dtfi),
                        Impacts = jsonImpacts,
                        Events = jsonEvents
                    });
                }
            }
            else
            {
                foreach (var init in initiatives)
                {
                    var jsonEvents = new List<object>();
                    var jsonImpacts = new List<object>();

                    foreach (var e in init.Events)
                    {
                        jsonEvents.Add(new
                        {
                            ID = e.ID,
                            Type = e.PublishedEdit.Type.ToString(),
                            Date = e.PublishedEdit.DisplayDate.ToShortDateString(),
                            Branches = e.Branches.Select(b => b.ID),
                            Regions = e.Regions.Select(r => r.ID),
                            Text = e.PublishedEdit.TextE,
                            Hover = e.PublishedEdit.HoverE,
                            Show = e.Show
                        });
                    }

                    foreach (var impact in init.Impacts)
                    {
                        jsonImpacts.Add(new
                        {
                            Level = impact.Level,
                            Branches = impact.Branches.Select(b => b.ID),
                            Regions = impact.Regions.Select(r => r.ID)
                        });
                    }

                    jsonInitiatives.Add(new
                    {
                        ID = init.ID,
                        Name = init.NameE,
                        Description = init.DescriptionE,
                        StartDate = init.StartDate.ToShortDateString(),
                        EndDate = init.EndDate.ToShortDateString(),
                        Impacts = jsonImpacts,
                        Events = jsonEvents
                    });
                }

            }


            return Json(jsonInitiatives, JsonRequestBehavior.AllowGet);
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
