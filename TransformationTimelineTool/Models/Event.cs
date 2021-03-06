﻿using System;
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
    public enum Status
    {
        Draft, Pending, Approved
    }
    public class Event
    {

        private TimelineContext db = new TimelineContext();
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        
        public string CreatorID { get; set; }

        public Status Status { get; set; }


        [Display(Name = "Branches", ResourceType = typeof(Resources.Resources))]
        public string BranchesList
        {
            get
            {
                return string.Join("<br />", Branches.Select(b => b.Name).OrderBy(s => s));
            }
        }
        [Display(Name = "Regions", ResourceType = typeof(Resources.Resources))]
        public string RegionsList
        {
            get
            {
                return string.Join("<br />", Regions.Select(r => r.Name).OrderBy(s => s));
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
        public virtual User Creator { get; set; }
        public virtual ICollection<Edit> Edits { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}