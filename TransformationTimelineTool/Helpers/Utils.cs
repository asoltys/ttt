using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.Helpers
{
    public class Utils
    {
        public static string GetUserName(string FullIdentityName)
        {
            String[] name = FullIdentityName.Split('\\');
          
            return name[1];
        }

        public static List<User> GetOPIs()
        {
            TimelineContext db = new TimelineContext();
            UserManager<User> userManager;

            userManager = new UserManager<User>(new UserStore<User>(db));
            var opis = new List<User>();
            foreach (var user in db.Users.ToList<User>())
            {
                if (userManager.IsInRole(user.Id, "OPI"))
                {
                    opis.Add(user);
                }
            }

            return opis;
        }

        public static User GetCurrentUser()
        {
            TimelineContext db = new TimelineContext();
            UserManager<User> userManager;

            userManager = new UserManager<User>(new UserStore<User>(db));

            return userManager.FindByName(GetUserName(HttpContext.Current.User.Identity.Name));
        }
        
        public static string[] GetUserRoles(string FullIdentityName = "")
        {
            TimelineContext db = new TimelineContext();
            UserManager<User> userManager;
            RoleManager<IdentityRole> roleManager;

            userManager = new UserManager<User>(new UserStore<User>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            List<string> userRoles = new List<string>();
            User user;

            if (FullIdentityName == "")
            {
                 user = GetCurrentUser();
            }
            else
            {
                 user = userManager.FindByName(GetUserName(FullIdentityName));
            }

            if (user == null)
            {
                return new string[] { };
            }
            
            foreach (var role in user.Roles)
            {
                var myRole = roleManager.FindById(role.RoleId);
                userRoles.Add(myRole.Name);
                //Debug.WriteLine(myRole.Name);
            }

            return userRoles.ToArray();
        }
        public static string GetUsernameFromEmail(string email)
        {

            DirectoryEntry root = new DirectoryEntry(
                "LDAP://adldap.ncr.pwgsc.gc.ca/dc=ad,dc=pwgsc-tpsgc,dc=gc,dc=ca",
                "pacweb",
                "god!power");

            DirectorySearcher searcher = new DirectorySearcher(
                root,
                "(mail=" + email + ")");

            string username = "";
            SearchResult person;

            try
            {
                person = searcher.FindOne();
                username = person.Properties["mailNickname"][0].ToString();
            }
            catch
            {
                username = "";
            }

            //jsonData["Verified"] = true;
            return username;

        }

        public static string GetFullName()
        {
            var UserName = Utils.GetUserName(HttpContext.Current.User.Identity.Name);
            DirectoryEntry root = new DirectoryEntry(
               "LDAP://adldap.ncr.pwgsc.gc.ca/dc=ad,dc=pwgsc-tpsgc,dc=gc,dc=ca",
               "pacweb",
               "god!power");

            DirectorySearcher adSearcher = new DirectorySearcher(root);
            adSearcher.Filter = "(MAILNICKNAME=" + UserName + ")";
            SearchResult userObject = adSearcher.FindOne();
            var FullName = "";
            if (userObject != null)
            {
                string[] props = new string[] { "givenName", "SN" };
                foreach (string prop in props)
                {
                    FullName += userObject.Properties[prop][0] + " ";
                }
            }
            return FullName;
        }

        public static string GetNameFromEmail(string mail)
        {
            DirectoryEntry root = new DirectoryEntry(
               "LDAP://adldap.ncr.pwgsc.gc.ca/dc=ad,dc=pwgsc-tpsgc,dc=gc,dc=ca",
               "pacweb",
               "god!power");
            
            DirectorySearcher adSearcher = new DirectorySearcher(root);
            adSearcher.Filter = "(mail=" + mail + ")";
            SearchResult userObject = adSearcher.FindOne();
            var FullName = "";
            if (userObject != null)
            {
                string[] props = new string[] { "givenName", "SN" };
                foreach (string prop in props)
                {
                    FullName = String.Format("{0} : {1}", prop, userObject.Properties[prop][0]);
                    Utils.log(FullName);
                }
            }
            return FullName;
        }

        public static bool SendMail(String Recipient, String Subject, String Message, List<String> CC = null)
        {
            CC = CC ?? new List<String>();
            MailAddress FromAddress = new MailAddress("PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca", "TimelineTool");
            MailAddress RecipientAddress = new MailAddress(Recipient);
            MailMessage Mail = new MailMessage(FromAddress, RecipientAddress);
            Mail.Subject = Subject;
            Mail.IsBodyHtml = true;
            Mail.Body = Message;
            if (CC.Count() > 0)
                CC.ForEach(CopyAddress => Mail.CC.Add(new MailAddress(CopyAddress)));
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(Mail);
                return true;
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Utils\\SendMail() Error: " + ex.ToString());
                return false;
            }
        }

        public static void log(String Message)
        {
            System.Diagnostics.Debug.WriteLineIf(Message.Length > 0, Message);
        }
    }
}