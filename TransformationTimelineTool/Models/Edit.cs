using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

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
        [AllowHtml]
        [Display(Name = "EventEngText", ResourceType = typeof(Resources.Resources))]
        public String TextE { get; set; }

        [AllowHtml]
        [Display(Name = "EventFraText", ResourceType = typeof(Resources.Resources))]
        public String TextF { get; set; }

        [Display(Name = "EventEngHover", ResourceType = typeof(Resources.Resources))]
        public String HoverE { get; set; }

        [Display(Name = "EventFraHover", ResourceType = typeof(Resources.Resources))]
        public String HoverF { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        
        public bool Published { get; set; }
        public string Hover
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "fr")
                {
                    return HoverF;
                }
                else
                {
                    return HoverE;
                }
            }
        }
        public virtual Event Event { get; set; }
        public virtual User Editor { get; set; }
    }
}