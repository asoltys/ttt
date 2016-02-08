using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace TransformationTimelineTool.Models
{
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

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages),
            ErrorMessageResourceName = "EventRequiredDescriptionEnglish")]
        [Display(Name = "EventEngHover", ResourceType = typeof(Resources.Resources))]
        public String HoverE { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages),
            ErrorMessageResourceName = "EventRequiredDescriptionFrench")]
        [Display(Name = "EventFraHover", ResourceType = typeof(Resources.Resources))]
        public String HoverF { get; set; }

        [Required]
        [Display(Name = "Type", ResourceType = typeof(Resources.Resources))]
        public Type Type { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages),
            ErrorMessageResourceName = "EventRequiredDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date", ResourceType = typeof(Resources.Resources))]
        public DateTime DisplayDate { get; set; }

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