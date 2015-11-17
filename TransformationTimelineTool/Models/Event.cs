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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public String Text { get; set; }
        public String Hover { get; set; }
        public virtual Initiative Initiative { get; set; }
    }
}