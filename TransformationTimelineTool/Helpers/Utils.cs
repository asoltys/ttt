using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.Helpers
{
    public class Utils
    {
        public static string GetUserName()
        {
            if(HttpContext.Current.User.Identity.Name == "DESKTOP-164KO4D\\matty")
            {
                return "Matty";
            }

            string[] name = HttpContext.Current.User.Identity.Name.Split('\\');

            return name[1];
        }

        public static string GetUserName(string FullIdentityName)
        {
            String[] name = FullIdentityName.Split('\\');
          
            return name[1];
        }

        public static List<User> GetApprover()
        {
            TimelineContext db = new TimelineContext();
            UserManager<User> userManager;

            userManager = new UserManager<User>(new UserStore<User>(db));
            var opis = new List<User>();
            foreach (var user in db.Users.ToList<User>())
            {
                if (userManager.IsInRole(user.Id, "Approver"))
                {
                    opis.Add(user);
                }
            }

            return opis.OrderBy(o => o.UserName).ToList<User>();
        }

        public static User GetCurrentUser()
        {
            TimelineContext db = new TimelineContext();
            UserManager<User> userManager;

            userManager = new UserManager<User>(new UserStore<User>(db));

            return userManager.FindByName(GetUserName(HttpContext.Current.User.Identity.Name));
        }
        
        public static List<string> GetUserRoles(string FullIdentityName = "")
        {
            TimelineContext db = new TimelineContext();
            RoleManager<IdentityRole> roleManager;

            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            List<string> userRoles = new List<string>();
            User user;

            if (FullIdentityName == "")
            {
                 user = GetCurrentUser();
            }
            else
            {
                UserManager<User> userManager;
                userManager = new UserManager<User>(new UserStore<User>(db));
                user = userManager.FindByName(GetUserName(FullIdentityName));
            }

            if (user == null)
            {
                return userRoles;
            }
            
            foreach (var role in user.Roles)
            {
                var myRole = roleManager.FindById(role.RoleId);
                userRoles.Add(myRole.Name);
                //Debug.WriteLine(myRole.Name);
            }

            return userRoles;
        }

        public static string GetEmailFromUserName(string userName)
        {
            var searchString = "(mailNickname=" + userName + ")";
            DirectoryEntry root = new DirectoryEntry(
                "LDAP://adldap.ncr.pwgsc.gc.ca/dc=ad,dc=pwgsc-tpsgc,dc=gc,dc=ca",
                "pacweb",
                "god!power");

            DirectorySearcher searcher = new DirectorySearcher(
                root,
                searchString);

            string email = "";
            SearchResult person;

            try
            {
                person = searcher.FindOne();
                email = person.Properties["mail"][0].ToString();
            }
            catch
            {
                email = "";
            }

            return email;
        }

        public static string GetUsernameFromEmail(string email)
        {
            var mailAddress = new MailAddress(email);

            var pwEmailAliases = new List<string>
            {
                "pwgsc-tpsgc.gc.ca",
                "tpsgc-pwgsc.gc.ca",
                "pwgsc.gc.ca",
                "tpsgc.gc.ca",
            };

            var searchString = "";

            if (!pwEmailAliases.Contains(mailAddress.Host))
            {
                searchString = String.Format("(mail={0}@{1})", mailAddress.User, mailAddress.Host);
            }
            else
            {
                searchString = String.Format("(|(mail={0}@{1})(mail={0}@{2})(mail={0}@{3})(mail={0}@{4}))",
                                                mailAddress.User,
                                                "pwgsc-tpsgc.gc.ca",
                                                "tpsgc-pwgsc.gc.ca",
                                                "pwgsc.gc.ca",
                                                "tpsgc.gc.ca");
            }

            DirectoryEntry root = new DirectoryEntry(
                "LDAP://adldap.ncr.pwgsc.gc.ca/dc=ad,dc=pwgsc-tpsgc,dc=gc,dc=ca",
                "pacweb",
                "god!power");

            DirectorySearcher searcher = new DirectorySearcher(
                root,
                searchString);

            string username = "";
            SearchResult person;

            try
            {
                person = searcher.FindOne();
                username = person.Properties["mailNickname"][0].ToString();
            }
            catch (NullReferenceException nre)
            {
                throw nre;
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

        public static void SendMail(string Recipient, string Subject, string Message)
        {
            MailAddress FromAddress = new MailAddress("PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca", "TimelineTool");
            MailAddress RecipientAddress = new MailAddress(Recipient);
            using (MailMessage Mail = new MailMessage(FromAddress, RecipientAddress))
            using (SmtpClient client = new SmtpClient())
            {
                Mail.Subject = Subject;
                Mail.IsBodyHtml = true;
                Mail.Body = Message;
                client.Timeout = 10000;
                client.UseDefaultCredentials = true;
                try
                {
                    client.Send(Mail);
                }
                catch (Exception ex)
                {
                    log(ex.StackTrace);
                }
            }
        }

        public async static Task SendMailAsync(String Recipient, String Subject, String Message, List<String> CC = null)
        {
            Task completedTask = Task.FromResult(false);
            if (WebConfigurationManager.AppSettings["SendMail"] == "false")
                await completedTask;
            //log("SendMail was called");
            CC = CC ?? new List<String>();
            MailAddress FromAddress = new MailAddress("PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca", "TimelineTool");
            MailAddress RecipientAddress = new MailAddress(Recipient);
            using (MailMessage Mail = new MailMessage(FromAddress, RecipientAddress))
            using (SmtpClient client = new SmtpClient())
            {
                Mail.Subject = Subject;
                Mail.IsBodyHtml = true;
                Mail.Body = Message;
                if (CC.Count() > 0)
                    CC.ForEach(CopyAddress => Mail.CC.Add(new MailAddress(CopyAddress)));
                client.Timeout = 10000;
                client.UseDefaultCredentials = true;
                HttpContext.Current.AllowAsyncDuringSyncStages = true;
                try
                {
                    await client.SendMailAsync(Mail);
                } catch (Exception ex)
                {
                    Utils.log(ex.StackTrace);
                }
                HttpContext.Current.AllowAsyncDuringSyncStages = false;
            }
        }

        public static void log(String Message)
        {
            System.Diagnostics.Debug.WriteLineIf(Message.Length > 0, Message);
        }

        public static void log(List<string> list)
        {
            foreach (var element in list)
            {
                log(element);
            }
        }

        public static void log<T>(List<T> list)
        {
            var props = typeof(T).GetProperties();
            foreach (var element in list)
            {
                foreach (var prop in props)
                {
                    log(prop.Name + ": " + prop.GetValue(element, null));
                }
            }
        }

        public static void listLog(ICollection<Object> list)
        {
            foreach (var element in list)
                log(element.ToString());
        }
        
        public static string GetTranslation(String value)
        {
            try
            {
                return Resources.Resources.ResourceManager.GetString(value);
            } catch (Exception)
            {
                return "Resource Not Found";
            }
        }

    }
}