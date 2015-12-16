using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Category
    {
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}