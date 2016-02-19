using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Exceptions;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.ViewModels;

namespace TransformationTimelineTool.Controllers
{
    [Authorize(Roles = "Admin,Approver,Editor")]
    [RoutePrefix("Activites-Activities")]
    [Route("{action=index}")]
    public class ActivitiesController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        // GET: Events
        public ActionResult Index()
        {
            var currentUser = Utils.GetCurrentUser();
            var events = currentUser.Initiatives.SelectMany(i => i.Events).ToList<Event>();
            return View(events);
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
        [Route("Creer-Create")]
        public ActionResult Create(int? id)
        {
            var currentUser = Utils.GetCurrentUser();
            var viewModel = new EventViewModel();
            if (Thread.CurrentThread.CurrentCulture.Name == "fr")
            {
                viewModel.Branches = db.Branches.OrderBy(b => b.NameF).ToList<Branch>();
                viewModel.Regions = db.Regions.OrderBy(r => r.NameF).ToList<Region>();
                viewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList().OrderBy(i => i.NameF), "id", "Name", id);
            }
            else
            {
                viewModel.Branches = db.Branches.OrderBy(b => b.NameE).ToList<Branch>();
                viewModel.Regions = db.Regions.OrderBy(r => r.NameE).ToList<Region>();
                viewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList().OrderBy(i => i.NameE), "id", "Name", id);
            }
            return View(viewModel);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Creer-Create")]
        public ActionResult Create(EventViewModel eventViewModel,
            string[] selectedBranches,
            string[] selectedRegions)
        {
            var eventToCreate = eventViewModel.Event;
            var currentUser = Utils.GetCurrentUser();

            if (ModelState.IsValid)
            {
                try
                {
                    var StatusStates = Tuple.Create(Status.Draft, eventViewModel.Event.Status);
                    var NotificationAction = DetermineNotificationAction(StatusStates);
                    eventToCreate.CreatorID = currentUser.Id;

                    eventToCreate.Branches = new List<Branch>();
                    foreach (var branch in db.Branches)
                    {
                        if (selectedBranches.Contains(branch.ID.ToString()))
                        {
                            eventToCreate.Branches.Add(branch);
                        }
                    }

                    eventToCreate.Regions = new List<Region>();
                    foreach (var region in db.Regions)
                    {
                        if (selectedRegions.Contains(region.ID.ToString()))
                        {
                            eventToCreate.Regions.Add(region);
                        }
                    }

                    var edit = eventViewModel.Edit;

                    edit.EditorID = currentUser.Id;
                    edit.Date = DateTime.Now;

                    if (eventToCreate.Status == Status.Approved)
                    {
                        edit.Published = true;
                    }

                    eventToCreate.Edits = new List<Edit>();
                    eventToCreate.Edits.Add(edit);

                    db.Events.Add(eventToCreate);
                    db.SaveChanges();
                    HandleNotification(NotificationAction, eventToCreate.Edits);
                    return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            eventViewModel.Branches= db.Branches.ToList();
            eventViewModel.Regions = db.Regions.ToList();
            eventViewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList(), "id", "NameE", eventViewModel.InitiativeID);
            
            return View(eventViewModel);
        }

        // GET: Events/Edit/5
        [Route("Modifier-Edit")]
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
            var viewModel = new EventViewModel();
            if (Thread.CurrentThread.CurrentCulture.Name == "fr")
            {
                eventViewModel.InitiativeSelect = new SelectList(Utils.GetCurrentUser().Initiatives.ToList().OrderBy(i => i.NameF), "id", "Name", id);
            }
            else
            {
                eventViewModel.InitiativeSelect = new SelectList(Utils.GetCurrentUser().Initiatives.ToList().OrderBy(i => i.NameE), "id", "Name", id);
            }
            PopulateEventRegionsData(eventViewModel.Event);
            PopulateEventBranchesData(eventViewModel.Event);

            if (eventViewModel.Event == null)
            {
                return HttpNotFound();
            }

            ViewBag.InitiativeID = new SelectList(Utils.GetCurrentUser().Initiatives, "ID", "NameE", eventViewModel.Event.InitiativeID);
            return View(eventViewModel);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Modifier-Edit")]
        public ActionResult Edit(EventViewModel eventViewModel, string[] selectedRegions, string[] selectedBranches)
        {

            if (eventViewModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventToUpdate = db.Events
               .Where(i => i.ID == eventViewModel.Event.ID)
               .Single();

            var currentUser = Utils.GetCurrentUser();

            if (ModelState.IsValid)
            {
                try
                {
                    var StatusStates = Tuple.Create(eventToUpdate.Status, eventViewModel.Event.Status);
                    var NotificationAction = DetermineNotificationAction(StatusStates);
                    eventToUpdate.InitiativeID = eventViewModel.Event.InitiativeID;
                    eventToUpdate.Status = eventViewModel.Event.Status;

                    UpdateEventRegions(selectedRegions, eventToUpdate);
                    UpdateEventBranches(selectedBranches, eventToUpdate);

                    var edit = eventViewModel.Edit;

                    edit.DisplayDate = eventViewModel.Edit.DisplayDate;
                    edit.Type = eventViewModel.Edit.Type;
                    edit.Editor = db.Users.Find(currentUser.Id);
                    edit.Date = DateTime.Now;

                    if (eventToUpdate.Status == Status.Approved)
                    {
                        eventToUpdate.PublishedEdit.Published = false;
                        edit.Published = true;
                    }
                    eventToUpdate.Edits.Add(edit);
                    db.SaveChanges();
                    HandleNotification(NotificationAction, eventToUpdate.Edits);
                    return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateEventRegionsData(eventToUpdate);
            PopulateEventBranchesData(eventToUpdate);
            eventViewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList(), "id", "NameE", eventViewModel.InitiativeID);
            return View(eventViewModel);
        }

        public bool HandleNotification(string Action, ICollection<Edit> @edits, bool Debug = false)
        {
            var NumberOfEdits = @edits.Count();
            // Fetch previous Editor ID
            Edit PreviousEdit;
            if (NumberOfEdits <= 1)
            {
                PreviousEdit = @edits.OrderByDescending(e => e.Date).First();
            } else
            {
                PreviousEdit = @edits.OrderByDescending(e => e.Date).Skip(1).First();
            }
            var PreviousEditor = PreviousEdit.Editor;
            var CurrentUser = Utils.GetCurrentUser();
            if (Debug) Utils.log("Previous Edit ID: " + PreviousEdit.ID + ", Previous Editor: " + PreviousEdit.Editor.Email);

            var SendTo = "";
            var MailSubject = "";
            var MailBody = ""; // Format: {0}->Server name, {1}->Event ID, {2}->Admin email
            var ServerDomain = WebConfigurationManager.AppSettings["serverURL"];
            var AdminEmail = WebConfigurationManager.AppSettings["adminEmail"];
            var CopyList = new List<string>();
            switch (Action)
            {
                case "None":
                    return true;
                case "Pending":
                    SendTo = CurrentUser.Approver != null ? CurrentUser.Approver.Email : AdminEmail;
                    MailSubject = Resources.Resources.PendingMailSubject;
                    MailBody = Resources.Resources.PendingMailBody;
                    if (CurrentUser.Approver != null) CopyList.Add(AdminEmail);
                    CopyList.Add(CurrentUser.Email);
                    break;
                case "Approve":
                    SendTo = PreviousEditor.Email;
                    MailSubject = Resources.Resources.ApprovedMailSubject;
                    MailBody = Resources.Resources.ApprovedMailBody;
                    if (PreviousEditor.Approver != null) CopyList.Add(PreviousEditor.Approver.Email);
                    CopyList.Add(AdminEmail);
                    break;
                case "Reject":
                    SendTo = PreviousEditor.Email;
                    MailSubject = Resources.Resources.RejectMailSubject;
                    MailBody = Resources.Resources.RejectMailBody;
                    if (PreviousEditor.Approver != null) CopyList.Add(PreviousEditor.Approver.Email);
                    CopyList.Add(AdminEmail);
                    break;
                default:
                    throw new SendMailException("SendMail could not recognize the action");
            }
            MailBody = String.Format(MailBody, ServerDomain, PreviousEdit.EventID, AdminEmail, ServerDomain);
            if (Debug)
            {
                Utils.log("To: " + SendTo);
                Utils.log("Subject: " + MailSubject);
                Utils.log("Body" + MailBody);
            }
            Utils.SendMailAsync(SendTo, MailSubject, MailBody, CopyList);
            return true;
        }

        public string DetermineNotificationAction(Tuple<Status, Status> EventStatus, bool Debug = false)
        {
            string PreviousStatus = EventStatus.Item1.ToString();
            string NewStatus = EventStatus.Item2.ToString();
            List<string> CurrentUserRoles = Utils.GetCurrentUserRoles();
            bool IsAdmin = CurrentUserRoles.Contains("Admin");
            bool IsApprover = IsAdmin == true ? false : CurrentUserRoles.Contains("Approver");
            bool IsEditor = IsAdmin == true || IsApprover == true ? false : CurrentUserRoles.Contains("Editor");
            if (Debug)
            {
                Utils.log(PreviousStatus + " -> " + NewStatus);
                Utils.log("Admin: " + IsAdmin.ToString() + ", Editor: " + IsEditor.ToString() + ", Approver:" + IsApprover.ToString());
            }
            string None = "None";
            string Pending = "Pending";
            string Approve = "Approve";
            string Reject = "Reject";
            if (IsAdmin) return None;
            switch (PreviousStatus)
            {
                case "Draft":
                    switch (NewStatus)
                    {
                        case "Draft":
                            return None;
                        case "Pending":
                            if (IsEditor)
                            {
                                return Pending;
                            }
                            return None;
                        case "Approved":
                            // Editors cannot reach this block
                            return None;
                        default:
                            return None;
                    }
                case "Pending":
                    switch (NewStatus)
                    {
                        case "Draft":
                            if (IsApprover)
                            {
                                return Reject;
                            }
                            return None;
                        case "Pending":
                            return None;
                        case "Approved":
                            if (IsApprover)
                            {
                                return Approve;
                            }
                            return None;
                        default:
                            return None;
                    }
                case "Approved":
                    switch (NewStatus)
                    {
                        case "Draft":
                            return None;
                        case "Pending":
                            if (IsEditor)
                            {
                                return Pending;
                            }
                            return None;
                        case "Approved":
                            return None;
                        default:
                            return None;
                    }
                default:
                    return None;
            }
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
                    //Date = e.Date.ToShortDateString(),
                    //TextE = e.TextE,
                    //TextF = e.TextF,
                    //HoverE = e.HoverE,
                    //HoverF = e.HoverF,
                    InitiativeID = e.InitiativeID
                }), JsonRequestBehavior.AllowGet);
        }

        // GET: Events/Delete/5
        [Route("Supprimer-Delete")]
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
        [Route("Supprimer-Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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

        private void PopulateEventRegionsData(Event @event)
        {
            string Culture = Thread.CurrentThread.CurrentCulture.Name;
            var allRegions = Culture == "fr" ? db.Regions.OrderBy(r => r.NameF) : db.Regions.OrderBy(r => r.NameE);
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
            string Culture = Thread.CurrentThread.CurrentCulture.Name;
            var allBranches = Culture == "fr" ? db.Branches.OrderBy(b => b.NameF) : db.Branches.OrderBy(b => b.NameE);
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

        private MailMessage PrepareMessage(Event @event)
        {
            var body = @"<p>Hello {0}, you have an activity awaiting approval.</p>
                         <p>The ID of the Activity is {1}</p>
                         <p>Please click <a href='{2}/en/activities/edit/{1}'>here</a> to review the item.</p>";
            var message = new MailMessage();
            var to = "";
            var currentUser = Utils.GetCurrentUser();

            if (currentUser.Approver == null)
            {
                to = WebConfigurationManager.AppSettings["adminEmail"];
            }
            else
            {
                to = currentUser.Approver.Email;
            }
            message.To.Add(new MailAddress(to));
            message.From = new MailAddress("PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca", "TimelineTool");
            message.Subject = "New items ready for approval";
            message.Body = String.Format(body, currentUser.UserName, @event.ID, WebConfigurationManager.AppSettings["serverURL"]);
            message.IsBodyHtml = true;

            return message;
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
