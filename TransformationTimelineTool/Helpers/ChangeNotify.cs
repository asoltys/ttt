using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.Helpers
{
    public class ChangeNotify : IJob
    {
        List<Edit> ChangedEdits;
        List<Impact> ChangedImpact;
        List<Initiative> ChangedInitiatives;
        TimelineContext db = new TimelineContext();

        public void Execute(IJobExecutionContext context)
        {
            var edits = db.Edits.Where(e => e.Edited == true).ToList();
            var impacts = db.Impacts.Where(i => i.Edited == true).ToList();
            var initiatives = db.Initiatives.Where(i => i.Edited == true).ToList();
            Utils.log(edits);
        }

    }
}