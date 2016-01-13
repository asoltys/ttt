﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
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
        [Display(Name = "Type", ResourceType = typeof(Resources.Resources))]
        public Type Type { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date", ResourceType = typeof(Resources.Resources))]
        public DateTime Date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Pending Date")]
        public DateTime PendingDate { get; set; }


        [AllowHtml]
        [Display(Name = "EventEngText", ResourceType = typeof(Resources.Resources))]
        public String TextE { get; set; }
        [AllowHtml]
        [Display(Name = "Pending English Text")]
        public String PendingTextE { get; set; }

        [AllowHtml]
        [Display(Name = "EventFraText", ResourceType = typeof(Resources.Resources))]
        public String TextF { get; set; }
        [AllowHtml]
        [Display(Name = "Pending French Text")]
        public String PendingTextF { get; set; }

        [Display(Name = "EventEngHover", ResourceType = typeof(Resources.Resources))]
        public String HoverE { get; set; }

        [Display(Name = "Pending English Hover")]
        public String PendingHoverE { get; set; }

        [Display(Name = "EventFraHover", ResourceType = typeof(Resources.Resources))]
        public String HoverF { get; set; }

        [Display(Name = "Pending French Hover")]
        public String PendingHoverF { get; set; }

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

        public Edit LatestEdit { get
            {
                if (Edits == null || Edits.Count == 0)
                {
                    return new Edit();
                }

                return Edits.OrderByDescending(e => e.Date).First();
            }
        }

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

        public string TextEDisplay
        {
            get
            {
                if(String.IsNullOrEmpty(TextE))
                {
                    return "[No Content]";
                }
                else
                {
                    return TextE;
                }
            }
        }

        public string TextFDisplay
        {
            get
            {
                if (String.IsNullOrEmpty(TextF))
                {
                    return "[No Content]";
                }
                else
                {
                    return TextF;
                }
            }
        }

        public string HoverEDisplay
        {
            get
            {
                if (String.IsNullOrEmpty(HoverE))
                {
                    return "[No Content]";
                }
                else
                {
                    return HoverE;
                }
            }
        }

        public string HoverFDisplay
        {
            get
            {
                if (String.IsNullOrEmpty(HoverF))
                {
                    return "[No Content]";
                }
                else
                {
                    return HoverF;
                }
            }
        }
        

        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Edit> Edits { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}