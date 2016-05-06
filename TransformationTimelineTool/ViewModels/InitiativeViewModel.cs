using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class InitiativeViewModel
    {
        private TimelineContext db = new TimelineContext();
        public int ID { get; set; }

        [Required]
        [Display(Name = "InitEnglishName", ResourceType = typeof(Resources.Resources))]
        public string NameE { get; set; }

        [Required]
        [Display(Name = "InitFrenchName", ResourceType = typeof(Resources.Resources))]
        public string NameF { get; set; }

        public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "fr")
                {
                    return NameF;
                }
                else
                {
                    return NameE;
                }
            }
        }

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

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages),
            ErrorMessageResourceName = "InitiativeTimelineRequired")]
        [AllowHtml]
        [Display(Name = "Timeline", ResourceType = typeof(Resources.Resources))]
        public string Timeline { get; set; }

        [AllowHtml]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdated { get; set; }

        public IEnumerable<User> Users { get; set; }

        public InitiativeViewModel(int? id) : base()
        {
            Initiative initiative = db.Initiatives.Find(id);

            ID = initiative.ID;
            NameE = initiative.NameE;
            NameF = initiative.NameF;            
            DescriptionE = initiative.DescriptionE;
            DescriptionF = initiative.DescriptionF;
            StartDate = initiative.StartDate;
            EndDate = initiative.EndDate;
            Timeline = initiative.Timeline;
            Users = db.Users.ToList<User>();
        }
        public InitiativeViewModel() : base()
        {

        }

        public Initiative BindToModel(Initiative initiativeToUpdate)
        {
            initiativeToUpdate.ID = ID;
            initiativeToUpdate.NameE = NameE;
            initiativeToUpdate.NameF = NameF;
            initiativeToUpdate.DescriptionE = DescriptionE;
            initiativeToUpdate.DescriptionF = DescriptionF;
            initiativeToUpdate.StartDate = StartDate;
            initiativeToUpdate.EndDate = EndDate;
            initiativeToUpdate.Timeline = Timeline;
            return initiativeToUpdate;

        }
    }
}