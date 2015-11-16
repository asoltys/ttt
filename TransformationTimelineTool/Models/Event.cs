using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Event
    {
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public DateTime Date { get; set; }
        public String Text { get; set; }
        public String Hover { get; set; }
        public virtual Initiative Initiative { get; set; }
    }
}