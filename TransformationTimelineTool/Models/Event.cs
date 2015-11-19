using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public enum Type
    {
        Class, Milestone
    }
    public class Event
    {
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public int BranchID { get; set; }
        public int RegionID { get; set; }
        public Type Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "English Text")]
        public String TextE { get; set; }
        [Display(Name = "French Text")]
        public String TextF { get; set; }
        [Display(Name = "English Hover")]
        public String HoverE { get; set; }
        [Display(Name = "French Hover")]
        public String HoverF { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Region Region { get; set; }
    }
}