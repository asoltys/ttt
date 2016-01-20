using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class EventJSON
    {
        private TimelineContext db = new TimelineContext();
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        
        public String Type { get; set; }
        public String TextE { get; set; }
        
        public String TextF { get; set; }
        
        public String HoverE { get; set; }
        
        public String HoverF { get; set; }

        public String Date { get; set; }
        public Models.Status status { get; set; }
        public EventJSON(int? id) : base()
        {
            Event @event = db.Events.Find(id);
            Edit publishedEdit = new Edit();
            
            if (@event.Edits.Any(e => e.Published))
            {
                publishedEdit = @event.Edits.Single(e => e.Published == true);
            }

            ID = @event.ID;
            Type = publishedEdit.Type.ToString();
            Date = publishedEdit.DisplayDate.ToShortDateString();
            TextE = publishedEdit.TextE;
            HoverE = publishedEdit.HoverE;
            TextF = publishedEdit.TextF;
            HoverF = publishedEdit.HoverF;
            Branches = @event.Branches.Select(b => b.ID);
            Regions = @event.Regions.Select(r => r.ID);
            Show = @event.Show;
        }

        public bool Show { get; set; }
        
        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<Edit> Edits { get; set; }
        public virtual IEnumerable<int> Branches { get; set; }
        public virtual IEnumerable<int> Regions { get; set; }
    }
}