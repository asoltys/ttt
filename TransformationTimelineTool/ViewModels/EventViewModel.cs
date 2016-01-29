using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class EventViewModel
    {
        public EventViewModel()
        {
            TypeSelect = new SelectList(new[] {
                Resources.Resources.Milestone,
                Resources.Resources.Training
            });
            StatusSelect = new SelectList(new[]
            {
                Resources.Resources.Draft,
                Resources.Resources.Pending,
                Resources.Resources.Approved
            });
        }
        public SelectList InitiativeSelect { get; set; }
        public SelectList TypeSelect { get; set; }
        public SelectList StatusSelect { get; set; }

        private TimelineContext db = new TimelineContext();

        public Event Event { get; set; }
        public Edit Edit { get; set; }
        public IEnumerable<Branch> Branches { get; set; }
        public IEnumerable<Region> Regions { get; set; }

        [Required]
        public int InitiativeID { get; set; }
        public Edit GetLatestEdit()
        {
            if (Event == null)
            {
                return new Edit();
            }

            if (Event.Edits == null || Event.Edits.Count == 0)
            {
                 return new Edit();
            }
            else
            {
                return Event.Edits.OrderByDescending(e => e.Date).First();
            }
        }
    }
}