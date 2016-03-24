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
        private static List<Initiative> ChangedImpactsInitiatives;
        private static List<Initiative> ChangedEditsInitiatives;
        private static List<Initiative> ChangedInitiatives;
        private static List<Initiative> AllChangedInitiatives;
        private static List<Initiative> SubscriberChangedInitiatives;
        private static List<Initiative> SubscriberChangedEditsInitiatives;
        private static List<Initiative> SubscriberChangedImpactsInitiatives;
        private static List<Subscriber> Subscribers;
        private static List<EmailInitiative> EmailInits;
        TimelineContext db = new TimelineContext();

        public void Execute(IJobExecutionContext context)
        {
            ManualExecute();
        }

        public void ManualExecute()
        {
            ChangedEdits = db.Edits.Where(e => e.Edited == true).ToList();
            ChangedEditsInitiatives = ChangedEdits.Select(e => e.Event.Initiative).Distinct().ToList();

            ChangedImpacts = db.Impacts.Where(i => i.Edited == true).ToList();
            ChangedImpactsInitiatives = ChangedImpacts.Select(i => i.Initiative).Distinct().ToList();

            ChangedInitiatives = db.Initiatives.Where(i => i.Edited == true).ToList();
            AllChangedInitiatives = ChangedInitiatives.Union(ChangedImpactsInitiatives.Union(ChangedEditsInitiatives)).ToList();
            Subscribers = db.Subscribers.ToList();


            foreach( var init in AllChangedInitiatives)
            {
                var emailInit = new EmailInitiative();

            }

            string MailSubject = "There has been some changes to events you subscribed.";

            foreach (var subscriber in Subscribers)
            {
                //var subscriberChangedInits = subscriber.Initiatives.Intersect(ChangedInitiatives);
                SubscriberChangedInitiatives = AllChangedInitiatives.Intersect(subscriber.Initiatives).ToList();
                SubscriberChangedEditsInitiatives = ChangedEditsInitiatives.Intersect(subscriber.Initiatives).ToList();
                SubscriberChangedImpactsInitiatives = ChangedImpactsInitiatives.Intersect(subscriber.Initiatives).ToList();

                string MailBody = GetMailBody();
                Utils.SendMail(subscriber.Email, MailSubject, MailBody);
            }
        }

        public void generateInitContent()
        {

        }

        private static string GetMailBody()
        {
            string Body = "<p>Following initiatives you have subscribed to have been changed since last week:</p>";
            foreach (var initiative in SubscriberChangedInitiatives)
            {
                Body += "<h2>Changes within initiative " + initiative.NameE + ":</h2>";
                if(ChangedInitiatives.Count > 0)
                {
                    foreach(var changedInitiative in ChangedInitiatives)
                    {
                        if(changedInitiative.ID == initiative.ID)
                        {
                            Body += "<p>The details for initiative " + initiative.NameE + " have changed.</p>";
                        }
                    }
                }

                if (SubscriberChangedEditsInitiatives.Contains(initiative))
                {
                    Body += "<p>The following activities within initiative " + initiative.NameE + " have changed.</p>";
                    Body += "<ul>";
                    foreach (var edit in ChangedEdits)
                    {
                        if (edit.Event.InitiativeID == initiative.ID)
                        {
                            Body += "<li>The Activity " + edit.HoverE + " has changed.</li>";
                        }
                    }

                    Body += "</ul>";
                }

                if (SubscriberChangedImpactsInitiatives.Contains(initiative))
                {
                    Body += "<p>The following impacts within initiative " + initiative.NameE + " have changed.</p>";
                    Body += "<ul>";
                    foreach (var impact in ChangedImpacts)
                    {
                        if (impact.InitiativeID == initiative.ID)
                        {
                            Body += "<li>The impact for " + impact.Initiative.NameE + " has changed. </li>";
                        }
                    }
                    Body += "</ul>";
                }
            }

            Body += "<p>Following initiatives you have subscribed to have been changed since last week:</p>";
            foreach (var initiative in SubscriberChangedInitiatives)
            {
                Body += "<h2>[FRENCH]Changes within initiative " + initiative.NameF + ":</h2>";
                if(ChangedInitiatives.Count > 0)
                {
                    foreach(var changedInitiative in ChangedInitiatives)
                    {
                        if(changedInitiative.ID == initiative.ID)
                        {
                            Body += "<p>[FRENCH]The details for initiative " + initiative.NameF + " have changed.</p>";
                        }
                    }
                }

                if (SubscriberChangedEditsInitiatives.Contains(initiative))
                {
                    Body += "<p>[FRENCH]The following activities within initiative " + initiative.NameF + " have changed.</p>";
                    Body += "<ul>";
                    foreach (var edit in ChangedEdits)
                    {
                        if (edit.Event.InitiativeID == initiative.ID)
                        {
                            Body += "<li>[FRENCH]The Activity " + edit.HoverF + " has changed.</li>";
                        }
                    }

                    Body += "</ul>";
                }

                if (SubscriberChangedImpactsInitiatives.Contains(initiative))
                {
                    Body += "<p>[FRENCH]The following impacts within initiative " + initiative.NameF + " have changed.</p>";
                    Body += "<ul>";
                    foreach (var impact in ChangedImpacts)
                    {
                        if (impact.InitiativeID == initiative.ID)
                        {
                            Body += "<li>[FRENCH]The impact for " + impact.Initiative.NameF + " has changed. </li>";
                        }
                    }
                    Body += "</ul>";
                }
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