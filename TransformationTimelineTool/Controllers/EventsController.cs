using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Exceptions;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.ViewModels;

namespace TransformationTimelineTool.Controllers
{
    [Authorize(Roles = "Admin,OPI,Editor")]
    public class EventsController : BaseController
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
        public ActionResult Create(int? id)
        {
            var currentUser = Utils.GetCurrentUser();
            var viewModel = new EventViewModel();
            viewModel.Branches= db.Branches.ToList<Branch>();
            viewModel.Regions = db.Regions.ToList<Region>();

            if (id != null)
            {
                viewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList(), "id", "NameE", id);
            }
            else
            {
                viewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList(), "id", "NameE");
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
            var currentUser = Utils.GetCurrentUser();

            if (ModelState.IsValid)
            {
                try
                {

                    eventToCreate.CreatorID = currentUser.Id;
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

                    eventViewModel.Edit.Editor = db.Users.Find(currentUser.Id);
                    eventViewModel.Edit.Date = DateTime.Now;

                    if (eventToCreate.Status == Status.Approved)
                    {
                        eventViewModel.Edit.Published = true;
                    }
                    eventToCreate.Edits.Add(eventViewModel.Edit);

                    db.Events.Add(eventToCreate);
                    db.SaveChanges();
                    if (!SendMail(eventToCreate)) throw new SendMailException("Something unexpected happened.");

                    //@event.Edit = edit;;

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                } catch (SendMailException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            eventViewModel.Branches= db.Branches.ToList();
            eventViewModel.Regions = db.Regions.ToList();
            eventViewModel.InitiativeSelect = new SelectList(currentUser.Initiatives.ToList(), "id", "NameE", eventViewModel.InitiativeID);
            
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
            eventViewModel.InitiativeSelect = new SelectList(Utils.GetCurrentUser().Initiatives.ToList<Initiative>(), "id", "NameE");

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
                    eventToUpdate.Status = eventViewModel.Event.Status;

                    UpdateEventRegions(selectedRegions, eventToUpdate);
                    UpdateEventBranches(selectedBranches, eventToUpdate);

                    var edit = eventViewModel.Edit;
                    var currentUser = Utils.GetCurrentUser();

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
                    if (!SendMail(eventToUpdate)) throw new SendMailException("Something unexpected happened.");
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
                catch (SendMailException ex)
                {
                    ModelState.AddModelError("SendMail", ex.Message);
                }
            }

            PopulateEventRegionsData(eventToUpdate);
            return View(eventToUpdate);
        }

        public bool SendMail(Event @event)
        {
            if (WebConfigurationManager.AppSettings["SendMail"] == "false") return true;
            var CurrentUser = Utils.GetCurrentUser();
            var Creator = db.Users.Find(@event.CreatorID);
            if (Creator.Id == null || Creator.ApproverID == null)
                throw new SendMailException("Either Creator Id is NULL or Approver Id is NULL");
            var SendTo = "";
            var MailSubject = "";
            var MailBody = ""; // Format: {0}->Server name, {1}->Event ID, {2}->Admin email, {3}->Timeline Tool URL
            var ServerDomain = WebConfigurationManager.AppSettings["serverURL"];
            var AdminEmail = WebConfigurationManager.AppSettings["adminEmail"];
            var TimelineToolURL = Resources.Resources.ApplicationHomeURL;
            var CopyList = new List<string>();

            if (CurrentUser.Id == Creator.Id && @event.Status == Status.Draft)
            {
                // Logic: Creator has saved a draft of an event
                // Action: Do not send any notification
                return true;
            } else if (CurrentUser.Id == Creator.Id && @event.Status == Status.Pending)
            {
                // Logic: Creator just submitted the event to be approved by an approver
                // Action: Send a mail to the Approver, CC Creator & Admin
                SendTo = Creator.Approver.Email;
                MailSubject = Resources.Resources.PendingMailSubject;
                MailBody = Resources.Resources.PendingMailBody;
                MailBody = String.Format(MailBody,
                    ServerDomain, @event.ID, AdminEmail, ServerDomain + TimelineToolURL);
                CopyList.Add(Creator.Email);
                CopyList.Add(AdminEmail);
            } else if (CurrentUser.Id == Creator.ApproverID && @event.Status == Status.Draft)
            {
                // Logic: Approver has rejected the event creation
                // Action: Send a mail to the Creator, CC Approver & Admin
                SendTo = Creator.Email;
                MailSubject = Resources.Resources.RejectMailSubject;
                MailBody = Resources.Resources.RejectMailBody;
                MailBody = String.Format(MailBody,
                    ServerDomain, @event.ID, AdminEmail, ServerDomain + TimelineToolURL);
                CopyList.Add(Creator.Approver.Email);
                CopyList.Add(AdminEmail);
            } else if (CurrentUser.Id == Creator.ApproverID && @event.Status == Status.Approved)
            {
                // TODO: Test this case where Approver is the Creator and accepts the change right away
                // Logic: Approver has approved the event creation / edit
                // Action: Send a mail to the Creator, CC Approver & Admin
                SendTo = Creator.Email;
                MailSubject = Resources.Resources.ApprovedMailSubject;
                MailBody = Resources.Resources.ApprovedMailBody;
                MailBody = String.Format(MailBody,
                    ServerDomain, @event.ID, AdminEmail, ServerDomain + TimelineToolURL);
                CopyList.Add(Creator.Approver.Email);
                CopyList.Add(AdminEmail);
            } else if (CurrentUser.Id == Creator.ApproverID && @event.Status == Status.Pending)
            {
                // Logic: Approver has set the event state to pending
                // Action: Do not send any notification
                return true;
            } else
            {
                SendTo = AdminEmail;
                MailSubject = "Something unexpected happened with an event";
                MailBody = Resources.Resources.ApprovedMailBody;
                MailBody = String.Format(MailBody,
                    ServerDomain, @event.ID, AdminEmail, ServerDomain + TimelineToolURL);
            }
            if (!Utils.SendMail(SendTo, MailSubject, MailBody, CopyList))
                throw new SendMailException("Please check your server settings. SMTP client has failed to send the emails");
            return true;
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

        private MailMessage PrepareMessage(Event @event)
        {
            var body = @"<p>Hello {0}, you have an activity awaiting approval.</p>
                         <p>The ID of the Activity is {1}</p>
                         <p>Please click <a href='{2}/en/events/edit/{1}'>here</a> to review the item.</p>";
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
