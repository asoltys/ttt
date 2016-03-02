using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.Helpers
{
    public class ChangeNotify : IJob
    {
        private static List<Edit> ChangedEdits;
        private static List<Impact> ChangedImpacts;
        private static List<Initiative> ChangedInitiatives;
        TimelineContext db = new TimelineContext();

        public void Execute(IJobExecutionContext context)
        {
            ChangedEdits = db.Edits.Where(e => e.Edited == true).ToList();
            ChangedImpacts = db.Impacts.Where(i => i.Edited == true).ToList();
            ChangedInitiatives = db.Initiatives.Where(i => i.Edited == true).ToList();

            List<string> Recipients = GetRecipients();
            string MailSubject = "There has been some changes to events you subscribed.";
            string MailBody = GetMailBody();

            foreach (var recipient in Recipients)
            {
                Utils.SendMail(recipient, MailSubject, MailBody);
            }
        }

        private static string GetMailBody()
        {
            string Body = "Following list has been changed since last week. <br>";
            foreach (var edit in ChangedEdits)
            {
                Body += "";
            }
            foreach (var initiative in ChangedInitiatives)
            {
                Body += "The initiative " + initiative.NameE + " has changed. <br>";
            }
            foreach (var impact in ChangedImpacts)
            {
                Body += "The impact for " + impact.Initiative.NameE + " has changed. <br>";
            }
            return Body;
        }

        private static List<string> GetRecipients()
        {
            var email = "Ryan.Seo@pwgsc-tpsgc.gc.ca";
            List<string> EmailList = new List<string>();

            // Mass email send testing can be done here; just increase conditionals
            for (var i = 0; i < 1; i++)
            {
                EmailList.Add(email);
            }
            return EmailList;
        }

    }
}