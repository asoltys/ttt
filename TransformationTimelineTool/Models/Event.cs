using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Event
    {
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public int BranchID { get; set; }
        public int RegionID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public String TextE { get; set; }
        public String TextF { get; set; }
        public String HoverE { get; set; }
        public String HoverF { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Region Region { get; set; }
    }
}