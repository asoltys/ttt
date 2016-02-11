using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;

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

        public ActionResult Accessibility()
        {
            ViewBag.Message = "Accessibility.";

            return View();
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

        public ActionResult SetCulture(string lang)
        {
            var Referrer = Request.UrlReferrer.ToString();
            var ReferrerRequest = new HttpRequest(null, Referrer, null);
            var HostName = ReferrerRequest.Url.GetLeftPart(UriPartial.Authority);
            var AbsolutePath = ReferrerRequest.Url.AbsolutePath;
            var ReferrerQuery = ReferrerRequest.Url.Query;
            var QueryPattern = @"(lang=)([A-z]{3})";
            var NextQuery = Regex.Replace(ReferrerQuery, QueryPattern, "$1" + lang);
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
    }
}