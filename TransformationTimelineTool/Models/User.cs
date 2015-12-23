using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Region> Regions { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }

    }
}