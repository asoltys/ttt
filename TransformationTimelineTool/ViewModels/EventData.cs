using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class EventData
    {
        private TimelineContext db = new TimelineContext();
        public int ID { get; set; }
        public int InitiativeID { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resources.Resources))]
        public Models.Type Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date", ResourceType = typeof(Resources.Resources))]
        public DateTime Date { get; set; }
        public Status Status { get; set; }


        [Display(Name = "Branches", ResourceType = typeof(Resources.Resources))]
        public string BranchesList
        {
            get
            {
                return string.Join(" - ", Branches.Select(b => b.NameShort));
            }
        }
        [Display(Name = "Regions", ResourceType = typeof(Resources.Resources))]
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
                if (Edits == null)
                {
                    return false;
                }
                var temp = Edits.Any(e => e.Published);
                if (temp)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Edit PublishedEdit
        {
            get
            {
                var temp = Edits.Any(e => e.Published);
                if (!temp)
                {
                    return new Edit();
                }

                return Edits.Single(e => e.Published == true);

            }
        }

        public Edit LatestEdit
        {
            get
            {
                if (Edits == null || Edits.Count == 0)
                {
                    return new Edit();
                }
                else
                {
                    return Edits.OrderByDescending(e => e.Date).First();
                }

            }
        }

        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Edit> Edits { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}