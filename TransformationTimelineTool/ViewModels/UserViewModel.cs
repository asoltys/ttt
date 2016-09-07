using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class UserViewModel : IdentityUser
    {
        public UserViewModel() {}

        public UserViewModel(User user, UserManager<User> um)
        {
            this.Initiatives = user.Initiatives;
            this.UserRoles = String.Join(" - ", um.GetRoles(user.Id));
            this.Approver = user.Approver;
            this.Region = user.Region;
        }
        
        public IEnumerable<IdentityRole> Roles { get; set; }
        public IEnumerable<RolesData> PopulatedRoles{ get; set; }
        public IEnumerable<Initiative> Initiatives { get; set; }
        public IEnumerable<InitiativeData> PopulatedInitiatives { get; set; }
        public SelectList ApproverSelect { get; set; }
        public User User { get; set; }
        public virtual User Approver { get; set; }
        public List<User> Admins { get; set; }
        public List<User> Executives { get; set; }
        public List<User> Approvers { get; set; }
        public List<User> Editors { get; set; }
        public string UserRoles { get; set; }
        public SelectList RegionSelect { get; set; }
        public virtual Region Region { get; set; }

    }
}