using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Helpers;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class EventData
    {
        private TimelineContext db = new TimelineContext();

        public Event Event { get; set; }
        public Edit Edit { get; set; }
        public IEnumerable<Branch> Branches { get; set; }
        public IEnumerable<Region> Regions { get; set; }
        public IEnumerable<Initiative> Initiatives { get; set; }

        public Edit PrepareEventEdit()
        {

            var currentUser = Utils.GetCurrentUser();

            return new Edit
            {
                Editor = db.Users.Find(currentUser.Id),
                Date = DateTime.Now,
                Event = Event,
                TextE = Edit.TextE,
                TextF = Edit.TextF,
                HoverE = Edit.HoverE,
                HoverF = Edit.HoverF,
                Status = Status.Created
            };
            
        }
    }
}