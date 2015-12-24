using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public enum Status
    {
        Created, Pending, Approved
    }
    public class Edit
    {
        public int ID { get; set; }
        public string EditorID { get; set; }
        public int EventID { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public virtual Event Event { get; set; }
        public virtual User Editor { get; set; }
    }
}