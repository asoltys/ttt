using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.ViewModels;

namespace TransformationTimelineTool.Controllers
{
    public class HomeController : BaseController
    {
        private TimelineContext db = new TimelineContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Route("Accessibility")]
        public ActionResult Accessibility()
        {
            ViewBag.Message = "Accessibility.";

            return View();
        }

        [Route("Sabonner-Subscribe")]
        public ActionResult Subscribe()
        {
            var userName = Utils.GetUserName();
            var subscriber = db.Subscribers.SingleOrDefault(s => s.UserName == userName);
            if(subscriber == null)
            {
                subscriber = new Subscriber();
                subscriber.Initiatives = new List<Initiative>();
            }

            PopulateSubscriberInitiativeData(subscriber);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Sabonner-Subscribe")]
        public ActionResult Subscribe(string[] selectedInitiatives)
        {
            var userName = Utils.GetUserName();
            var addSubscriber = false;
            var subscriber = db.Subscribers.SingleOrDefault(s => s.UserName == userName);

            if (selectedInitiatives == null)
            {
                if(subscriber != null)
                {
                    db.Subscribers.Remove(subscriber);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
            }

            if(subscriber == null)
            {
                subscriber = new Subscriber
                {
                    UserName = userName,
                    Email = Utils.GetEmailFromUserName(userName)
                };

                subscriber.Initiatives = new List<Initiative>();
                addSubscriber = true;
            }

            UpdateSubscriberInitiatives(selectedInitiatives, subscriber);

            if (addSubscriber)
            {
                db.Subscribers.Add(subscriber);
            }

            db.SaveChanges();

            return RedirectToAction("Index", new { lang = Thread.CurrentThread.CurrentCulture.Name == "fr" ? "fra" : "eng" });
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SendMail(string to = "mathieu.wong-rose@pwgsc.gc.ca")
        {
            var body = "<p>Email from Matty Wong-Rose";
            var message = new MailMessage();

            message.To.Add(new MailAddress(to));
            message.From = new MailAddress("PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca", "TimelineTool");
            message.Subject = "New items ready for approval";
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
                return RedirectToAction("Index");
            }
        }

        [Route("sendNotification")]
        public ActionResult SendNotification()
        {
            ChangeNotify job = new ChangeNotify();
            job.ManualExecute();

            return RedirectToAction("Index");
        }

        [Route("clearEdited")]
        public ActionResult clearEdited()
        {
            var editedInits = db.Initiatives.Where(i => i.Edited == true);
            var editedEdits = db.Edits.Where(e => e.Edited == true);
            var editedImpacts = db.Impacts.Where(e => e.Edited == true);

            foreach( var init in editedInits)
            {
                init.Edited = false;
            }

            foreach( var edit in editedEdits)
            {
                edit.Edited = false;
            }

            foreach( var impact in editedImpacts)
            {
                impact.Edited = false;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult SetCulture(string lang)
        {
            var Referrer = Request.UrlReferrer.ToString();
            var ReferrerRequest = new HttpRequest(null, Referrer, null);
            var HostName = ReferrerRequest.Url.GetLeftPart(UriPartial.Authority);
            var AbsolutePath = ReferrerRequest.Url.AbsolutePath;
            var ReferrerQuery = ReferrerRequest.Url.Query;
            var QueryPattern = @"(lang=)([A-z]{3})";
            var MatchQuery = Regex.Match(ReferrerQuery, QueryPattern);
            string NextQuery;
            if (MatchQuery.Success)
                NextQuery = Regex.Replace(ReferrerQuery, QueryPattern, "$1" + lang);
            else
                NextQuery = "?lang=fra";
            return Redirect(HostName + AbsolutePath + NextQuery);
        }

        public ActionResult Playground()
        {
            var currentUser = Utils.GetCurrentUser();
            
            var eve = new Event
            {
                CreatorID = currentUser.Id,
                InitiativeID = 2,
                Status = Status.Approved,
                Branches = db.Branches.Where(b => b.ID == 5).ToList(),
                Regions = db.Regions.Where(r => r.ID <= 5).ToList()
            };

            var edit = new Edit
            {
                Date = DateTime.Now,
                DisplayDate = DateTime.Now,
                HoverE = "Add through events.add(edit)",
                HoverF = "Test",
                Published = true,
                Type = Models.Type.Milestone,
                EditorID = currentUser.Id
            };

            eve.Edits = new List<Edit>();
            eve.Edits.Add(edit);
            db.Events.Add(eve);
            //db.SaveChanges();

            return RedirectToAction("Index");
        }
        private void PopulateSubscriberInitiativeData(Subscriber subscriber)
        {
            string Culture = Thread.CurrentThread.CurrentCulture.Name;
            var allTTInitiatives = Culture == "fr" ? db.Initiatives.Where(i => i.Timeline == "TransformationTimeline").OrderBy(i => i.NameF) : db.Initiatives.Where(i => i.Timeline == "TransformationTimeline").OrderBy(r => r.NameE);
            var allBPInitiatives = Culture == "fr" ? db.Initiatives.Where(i => i.Timeline == "BP2020").OrderBy(i => i.NameF) : db.Initiatives.Where(i => i.Timeline == "BP2020").OrderBy(r => r.NameE);
            var subscriberInitiatives = new HashSet<int>(subscriber.Initiatives.Select(i => i.ID));
            var TTviewModel = new List<InitiativeData>();
            var BPviewModel2 = new List<InitiativeData>();

            foreach (var init in allTTInitiatives)
            {
                TTviewModel.Add(new InitiativeData
                {
                    ID = init.ID,
                    NameE = init.NameE,
                    NameF = init.NameF,
                    Flag = subscriberInitiatives.Contains(init.ID)
                });

            }
            foreach ( var init in allBPInitiatives)
            {
                BPviewModel2.Add(new InitiativeData
                {
                    ID = init.ID,
                    NameE = init.NameE,
                    NameF = init.NameF,
                    Flag = subscriberInitiatives.Contains(init.ID)
                });
            }

            ViewBag.Initiatives = TTviewModel;
            ViewBag.BP2020 = BPviewModel2;
        }
        private void UpdateSubscriberInitiatives(string[] selectedInitiatives, Subscriber subscriberToUpdate)
        {
            if (selectedInitiatives == null)
            {
                if (subscriberToUpdate.Initiatives != null || subscriberToUpdate.Initiatives.Count() > 0)
                    subscriberToUpdate.Initiatives.Clear();
                return;
            }

            var selectedInitiativesHS = new HashSet<string>(selectedInitiatives);
            var subscriberInitiatives = new HashSet<int>(subscriberToUpdate.Initiatives.Select(i => i.ID));

            foreach (var initiative in db.Initiatives)
            {
                if (selectedInitiativesHS.Contains(initiative.ID.ToString()))
                {
                    if (!subscriberInitiatives.Contains(initiative.ID))
                    {
                        subscriberToUpdate.Initiatives.Add(initiative);
                    }
                }
                else
                {
                    if (subscriberInitiatives.Contains(initiative.ID))
                    {
                        subscriberToUpdate.Initiatives.Remove(initiative);
                    }
                }
            }
        }
    }
}