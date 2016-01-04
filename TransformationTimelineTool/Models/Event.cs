using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;

namespace TransformationTimelineTool.Models
{
    public enum Type
    {
        Milestone, Training
    }
    public class Event
    {

        private TimelineContext db = new TimelineContext();
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public Type Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [AllowHtml]
        [Display(Name = "English Text")]
        public String TextE { get; set; }

        [Display(Name = "French Text")]
        [AllowHtml]
        public String TextF { get; set; }

        [Display(Name = "English Hover")]
        public String HoverE { get; set; }

        [Display(Name = "French Hover")]
        public String HoverF { get; set; }
        public string BranchesList
        {
            get
            {
                return string.Join(" - ", Branches.Select(b => b.NameShort));
            }
        }
        public string RegionsList
        {
            get
            {
                return string.Join(" - ", Regions.Select(r => r.NameShort));
            }
        }
        public bool Show
        {
            get
            {
                var result = db.Edits.Where(e => e.EventID == ID && e.Status == Status.Approved);

                if (result.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Edit> Edit { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}