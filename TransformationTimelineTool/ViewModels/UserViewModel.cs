using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<IdentityRole> Roles { get; set; }
        public IEnumerable<RolesData> PopulatedRoles{ get; set; }
        public IEnumerable<Initiative> Initiatives { get; set; }
        public IEnumerable<InitiativeData> PopulatedInitiatives { get; set; }
        public SelectList ApproverSelect { get; set; }
        public User User { get; set; }

        public List<User> Admins { get; set; }
        public List<User> Approvers { get; set; }
        public List<User> Editors { get; set; }
    }
}