using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace TransformationTimelineTool.Controllers
{
    public class HomeController : BaseController
    {
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

        public ActionResult SetCulture(string culture)
        {
            RouteData.Values["culture"] = culture;


            return RedirectToAction("Index","Initiatives");
            //return Redirect(Request.UrlReferrer.ToString());
        }
    }
}