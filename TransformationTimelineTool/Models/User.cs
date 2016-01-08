using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class User : IdentityUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }
        public string BranchesList
        {
            get
            {
                return string.Join(" - ", Branches.Select(b => b.NameShort));
            }
        }

        public string RegionsList
        {
            get
            {
                return string.Join(" - ", Regions.Select(r => r.NameShort));
            }
        }

        public virtual ICollection<Region> Regions { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }

        public IEnumerable<Event> Events
        {
            get
            {
                var userRegions = Regions.SelectMany(r => r.Events);
                var userBranchEvents = Branches.SelectMany(b => b.Events);
                return userBranchEvents.Intersect(userRegions);
            }
        }

    }
}