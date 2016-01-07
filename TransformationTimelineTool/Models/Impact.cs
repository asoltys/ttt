using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransformationTimelineTool.Models
{
    public enum Level
    {
        None, Low, Medium, High
    }
    public class Impact
    {
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public Level Level { get; set; }

        [AllowHtml]
        public string Justification { get; set; }
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
        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}