﻿using Microsoft.AspNet.Identity.EntityFramework;
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
        public IEnumerable<Branch> Branches { get; set; }
        public IEnumerable<BranchesData> PopulatedBranches { get; set; }
        public IEnumerable<Region> Regions { get; set; }
        public IEnumerable<RegionsData> PopulatedRegions { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public IEnumerable<RolesData> PopulatedRoles{ get; set; }
        public SelectList ApproverSelect { get; set; }
        public User User { get; set; }

    }
}