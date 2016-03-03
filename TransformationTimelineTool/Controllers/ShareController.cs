using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.Helpers;

namespace TransformationTimelineTool.Controllers
{
    [RoutePrefix("share")]
    public class ShareController : Controller
    {
        [Route("partial-form")]
        public ActionResult GetShareForm()
        {
            return PartialView("~/Views/Partials/ShareForm.cshtml");
        }

        [Route("submit-form")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> SubmitShareForm(FormCollection FC)
        {
            string Recipient = FC["email"];
            return await Send(Recipient);
        }

        private async Task<ActionResult> Send(string To)
        {
            List<string> ReturnValue = new List<string>();
            if (String.IsNullOrEmpty(To))
            {
                ReturnValue.Add("MailAddressEmpty");
                return Json(ReturnValue);
            }
            try
            {
                if (Regex.IsMatch(To, @"(\w+.\w+@)(?:pwgsc.gc.ca|pwgsc-tpsgc.gc.ca)"))
                {
                    await Utils.SendMailAsync(To, "Share Notification Test", "Share Notification Test");
                    ReturnValue.Add("MailSent");
                } else
                {
                    ReturnValue.Add("MailAddressInvalid");
                }
            } catch (RegexMatchTimeoutException)
            {
                ReturnValue.Add("MailAddressInvalid");
            }
            return Json(ReturnValue);
        }
    }
}