using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading;

namespace TransformationTimelineTool.Models
{
    public class Initiative
    {
        public int ID { get; set; }
        public string NCAExecID { get; set; }
        public string NCAEditorID { get; set; }
        public string PacExecID { get; set; }
        public string PacEditorID { get; set; }
        public string WstExecID { get; set; }
        public string WstEditorID { get; set; }
        public string OntExecID { get; set; }
        public string OntEditorID { get; set; }
        public string QueExecID { get; set; }
        public string QueEditorID { get; set; }
        public string AtlExecID { get; set; }
        public string AtlEditorID { get; set; }

        [Display(Name = "English Name")]
        public string NameE { get; set; }

        [Display(Name = "French Name")]
        public string NameF { get; set; } 
          
        [Display(Name = "English Description")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DescriptionE { get; set; }

        [Display(Name = "French Description")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DescriptionF { get; set; }

        [Display(Name = "Start")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public string Name
        {
            get
            {
                if(Thread.CurrentThread.CurrentCulture.Name == "fr")
                {
                    return NameF;
                }
                else
                {
                    return NameE;
                }
            }
        }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Impact> Impacts { get; set; }
        public virtual User NCAExec { get; set; }
        public virtual User NCAEditor { get; set; }
        public virtual User PacExec { get; set; }
        public virtual User PacEditor { get; set; }
        public virtual User WstExec { get; set; }
        public virtual User WstEditor { get; set; }
        public virtual User OntExec { get; set; }
        public virtual User OntEditor { get; set; }
        public virtual User QueExec { get; set; }
        public virtual User QueEditor { get; set; }
        public virtual User AtlExec { get; set; }
        public virtual User AtlEditor { get; set; }
    }
}