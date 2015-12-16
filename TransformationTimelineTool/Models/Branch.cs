using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Branch
    {
        public int ID { get; set; }
        public string NameShort { get; set; }
        [Display(Name = "English Name")]
        public string NameE { get; set; }
        [Display(Name = "French Name")]
        public string NameF { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Impact> Impacts { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}