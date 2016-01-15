using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        [Display(Name = "InitEnglishName", ResourceType = typeof(Resources.Resources))]
        public string NameE { get; set; }

        [Display(Name = "InitFrenchName", ResourceType = typeof(Resources.Resources))]
        public string NameF { get; set; }

        [Display(Name = "InitEnglishDesc", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DescriptionE { get; set; }

        [Display(Name = "InitFrenchDesc", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string DescriptionF { get; set; }

        [Display(Name = "Start", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public User NCAExec { get; set; }
        public  User NCAEditor { get; set; }
        public User PacExec { get; set; }
        public  User PacEditor { get; set; }
        public  User WstExec { get; set; }
        public  User WstEditor { get; set; }
        public  User OntExec { get; set; }
        public  User OntEditor { get; set; }
        public  User QueExec { get; set; }
        public  User QueEditor { get; set; }
        public  User AtlExec { get; set; }
        public  User AtlEditor { get; set; }
        public IEnumerable<User> Users { get; set; }

        public InitiativeViewModel(int? id) : base()
        {
            Initiative initiative = db.Initiatives.Find(id);

            ID = initiative.ID;
            //NCAExecID = initiative.NCAExecID;
            //NCAEditorID = initiative.NCAEditorID;
            //PacExecID = initiative.PacExecID;
            //PacEditorID = initiative.PacEditorID;
            //WstExecID = initiative.WstExecID;
            //WstEditorID = initiative.WstEditorID;
            //OntExecID = initiative.OntExecID;
            //OntEditorID = initiative.OntEditorID;
            //QueExecID = initiative.QueExecID;
            //QueEditorID = initiative.QueEditorID;
            //AtlExecID = initiative.AtlExecID;
            //AtlEditorID = initiative.AtlEditorID;
            NameE = initiative.NameE;
            NameF = initiative.NameF;            
            DescriptionE = initiative.DescriptionE;
            DescriptionF = initiative.DescriptionF;
            StartDate = initiative.StartDate;
            EndDate = initiative.EndDate;
            NCAExec = initiative.NCAExec;
            Users = db.Users.ToList<User>();
        }
        public InitiativeViewModel() : base()
        {

        }

        public Initiative BindToModel(Initiative initiativeToUpdate)
        {
            initiativeToUpdate.ID = ID;
            //initiativeToUpdate.NCAExecID = NCAExecID;
            //initiativeToUpdate.NCAEditorID = NCAEditorID;
            //initiativeToUpdate.PacExecID = PacExecID;
            //initiativeToUpdate.PacEditorID = PacEditorID;
            //initiativeToUpdate.WstExecID = WstExecID;
            //initiativeToUpdate.WstEditorID = WstEditorID;
            //initiativeToUpdate.OntExecID = OntExecID;
            //initiativeToUpdate.OntEditorID = OntEditorID;
            //initiativeToUpdate.QueExecID = QueExecID;
            //initiativeToUpdate.QueEditorID = QueEditorID;
            //initiativeToUpdate.AtlExecID = AtlExecID;
            //initiativeToUpdate.AtlEditorID = AtlEditorID;
            initiativeToUpdate.NameE = NameE;
            initiativeToUpdate.NameF = NameF;
            initiativeToUpdate.DescriptionE = DescriptionE;
            initiativeToUpdate.DescriptionF = DescriptionF;
            initiativeToUpdate.StartDate = StartDate;
            initiativeToUpdate.EndDate = EndDate;

            return initiativeToUpdate;

        }
    }
}