using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
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
    }
}