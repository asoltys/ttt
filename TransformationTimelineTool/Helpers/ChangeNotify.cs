using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
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
            ClearEdited();
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

            string MailSubject = "L'Outil de calendrier : Objectif 2020 et Transformation : Il y a des changements dans vos abonnements || Timeline Tool: Blueprint 2020 and Transformation: There are changes in your subscriptions";

            foreach (var subscriber in Subscribers)
            {
                //var subscriberChangedInits = subscriber.Initiatives.Intersect(ChangedInitiatives);
                SubscriberChangedInitiatives = AllChangedInitiatives.Intersect(subscriber.Initiatives).ToList();
                SubscriberChangedEditsInitiatives = ChangedEditsInitiatives.Intersect(subscriber.Initiatives).ToList();
                SubscriberChangedImpactsInitiatives = ChangedImpactsInitiatives.Intersect(subscriber.Initiatives).ToList();

                string MailBody = GetMailBody();
                string recipientEmail = subscriber.Email;
#if DEBUG
                recipientEmail = "PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca";
#endif
                Utils.SendMail(recipientEmail, MailSubject, MailBody);
            }
            string pacwebAddress = "PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca";
            string pacwebMailSubject = "Timeline Tool: Subscription Service Information";
            string pacwebBody = GenerateSubscriberInfo(Subscribers);
            Utils.SendMail(pacwebAddress, pacwebMailSubject, pacwebBody);
        }

        public string GenerateSubscriberInfo(List<Subscriber> subs)
        {
            string date = DateTime.Now.ToLongDateString();
            string time = DateTime.Now.ToLongTimeString();
            string body = "Server datetime: " + date + " - " + time + "<br/>";
            body += "Total number of subscribers: " + subs.Count() + "<br/><hr>";
            if (subs.Count > 0)
            {
                foreach (var sub in subs)
                {
                    body += sub.ID + ": " + sub.UserName + " (" + sub.Email + ")<br/>";
                }
            }
            body += "<hr>";
            return body;
        }

        public void ClearEdited()
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
        }

        public void generateInitContent()
        {

        }

        private static string GetMailBody()
        {
            var ServerDomain = WebConfigurationManager.AppSettings["serverURL"];
            string Body = "<p>Les initiatives auquelles vous vous êtes abonné a été mis à jour dans la dernière semaine:</p>";
            foreach (var initiative in SubscriberChangedInitiatives)
            {
                Body += "<h2>Changements dans l'initiative " + initiative.NameF + ":</h2>";
                if (ChangedInitiatives.Count > 0)
                {
                    foreach (var changedInitiative in ChangedInitiatives)
                    {
                        if (changedInitiative.ID == initiative.ID)
                        {
                            Body += "<p>La description de l'initiative a été changer.</p>";
                        }
                    }
                }

                if (SubscriberChangedEditsInitiatives.Contains(initiative))
                {
                    Body += "<p>Les activités suivantes ont été changer:</p>";
                    Body += "<ul>";
                    foreach (var edit in ChangedEdits)
                    {
                        if (edit.Event.InitiativeID == initiative.ID)
                        {
                            Body += "<li>" + edit.HoverF + "</li>";
                        }
                    }
                    Body += "</ul>";
                }
                /*
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
                }*/
            }
            Body += "<a href='"+ ServerDomain + "/Sabonner-Subscribe?lang=fra'>Cliquez ici pour vous déabonner ou de modifier votre abonnement.</a><br/>";
            Body += "Avez-vous des questions? <a href='mailto:teresa.martin@pwgsc-tpsgc.gc.ca'>Cliquez ici pour envoyer un courriel.</a>";
            Body += "<hr>";

            Body += "<p>The initiatives that you have subscribed to has been updated since last week:</p>";
            foreach (var initiative in SubscriberChangedInitiatives)
            {
                Body += "<h2>Changes within initiative " + initiative.NameE + ":</h2>";
                if(ChangedInitiatives.Count > 0)
                {
                    foreach(var changedInitiative in ChangedInitiatives)
                    {
                        if(changedInitiative.ID == initiative.ID)
                        {
                            Body += "<p>The initiative description has changed.</p>";
                        }
                    }
                }

                if (SubscriberChangedEditsInitiatives.Contains(initiative))
                {
                    Body += "<p>The following activities have changed:</p>";
                    Body += "<ul>";
                    foreach (var edit in ChangedEdits)
                    {
                        if (edit.Event.InitiativeID == initiative.ID)
                        {
                            Body += "<li>" + edit.HoverE + "</li>";
                        }
                    }
                    Body += "</ul>";
                }
                /*
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
                }*/
            }
            Body += "<a href='"+ ServerDomain + "/Sabonner-Subscribe?lang=eng'>Click here to unsubscribe or to change your subscription.</a><br/>";
            Body += "Questions? <a href='mailto:teresa.martin@pwgsc-tpsgc.gc.ca'>Click here to send an email.</a>";
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