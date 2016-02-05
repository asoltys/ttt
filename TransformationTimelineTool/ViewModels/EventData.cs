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

        public Models.Type Type { get; set; }

        public DateTime Date { get; set; }

        public bool Show
        {
            get
            {
                if (Edits == null) { return false; }

                //TODO Replace with return Edits.Any(e => e.Published);
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