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
        List<Edit> ChangedEdits;
        List<Impact> ChangedImpact;
        List<Initiative> ChangedInitiatives;
        TimelineContext db = new TimelineContext();

        public void Execute(IJobExecutionContext context)
        {
            var edits = db.Edits.Where(e => e.Edited == true).ToList();
            var impacts = db.Impacts.Where(i => i.Edited == true).ToList();
            var initiatives = db.Initiatives.Where(i => i.Edited == true).ToList();

            List<string> Recipients = GetRecipients();
            foreach (var recipient in Recipients)
            {
                Utils.SendMail(recipient, "ChangeNotify test", "test");
            }
        }

        private static List<string> GetRecipients()
        {
            var email = "Ryan.Seo@pwgsc-tpsgc.gc.ca";
            List<string> EmailList = new List<string>();

            // Mass email send testing can be done here; just increase conditionals
            for (var i = 0; i < 50; i++)
            {
                EmailList.Add(email);
            }
            return EmailList;
        }

    }
}