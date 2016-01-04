using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
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

            List<String> userRoles = new List<String>();
            ICollection<IdentityUserRole> roles;
            if (FullIdentityName == "")
            {
                roles = userManager.FindByName(GetCurrentUser().UserName).Roles;
            }
            else
            {
                roles = userManager.FindByName(GetUserName(FullIdentityName)).Roles;

            }

            foreach (var role in roles)
            {
                var myRole = roleManager.FindById(role.RoleId);
                userRoles.Add(myRole.Name);
                //Debug.WriteLine(myRole.Name);
            }

            return userRoles.ToArray();
        }
    }
}