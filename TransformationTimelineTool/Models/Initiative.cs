using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading;
using Resources;

namespace TransformationTimelineTool.Models
{
    public class Initiative
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "InitEnglishName", ResourceType = typeof(Resources.Resources))]
        public string NameE { get; set; }

        [Required]
        [Display(Name = "InitFrenchName", ResourceType = typeof(Resources.Resources))]
        public string NameF { get; set; }

        [Required]
        [Display(Name = "InitEnglishDesc", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DescriptionE { get; set; }

        [Required]
        [Display(Name = "InitFrenchDesc", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DescriptionF { get; set; }

        [Required]
        [Display(Name = "Start", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End", ResourceType = typeof(Resources.Resources))]
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
        public string Description
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "fr")
                {
                    return DescriptionF;
                }
                else
                {
                    return DescriptionE;
                }
            }
        }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Impact> Impacts { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}