using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
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
        [Display(Name = "Type", ResourceType = typeof(Resources.Resources))]
        public Type Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date", ResourceType = typeof(Resources.Resources))]
        public DateTime Date { get; set; }

       
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
                if (Edits == null || Edits.Count == 0)
                {
                    return false;
                }

                if (Edits.Any(e => e.Status == Status.Approved))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string TextE
        {
            get
            {
                if (!String.IsNullOrEmpty(LatestPublished.TextE))
                {
                    return LatestPublished.TextE;
                }
                else
                {
                    return "";
                }
            }
        }
        public string TextF
        {
            get
            {
                if (!String.IsNullOrEmpty(LatestPublished.TextF))
                {
                    return LatestPublished.TextF;
                }
                else
                {
                    return "";
                }
            }
        }

        public string HoverE
        {
            get
            {
                if (!String.IsNullOrEmpty(LatestPublished.HoverE))
                {
                    return LatestPublished.HoverE;
                }
                else
                {
                    return "";
                }
            }
        }
        public string HoverF
        {
            get
            {
                if (!String.IsNullOrEmpty(LatestPublished.HoverF))
                {
                    return LatestPublished.HoverF;
                }
                else
                {
                    return "";
                }
            }
        }

        public Edit LatestPublished
        {
            get
            {
                if (Edits == null || Edits.Count == 0)
                {
                    return new Edit();
                }

                if (Show)
                {
                    return Edits.Where(e => e.Status == Status.Approved)
                        .OrderByDescending(e => e.Date)
                        .First();
                }
                else
                {
                    return new Edit();
                }


            }
        }

        public Edit LatestEdit { get
            {
                if (Edits == null || Edits.Count == 0)
                {
                    return new Edit();
                }

                return Edits.OrderByDescending(e => e.Date).First();
            }
        }

        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Edit> Edits { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}