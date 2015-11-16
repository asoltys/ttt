using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.DAL
{
    public class TimelineInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<TimelineContext>
    {
        protected override void Seed(TimelineContext context)
        {
            var regions = new List<Region>
            {
                new Region {NameShort = "pac", Name = "Pacific" },
                new Region {NameShort = "wst", Name = "Western" },
                new Region {NameShort = "ont", Name = "Ontario" }
            };

            regions.ForEach(r => context.Regions.Add(r));
            context.SaveChanges();

            var branches = new List<Branch>
            {
                new Branch {NameShort = "rp", Name = "Real Propert" },
                new Branch {NameShort = "ciob", Name = "Chief Information Officer Branch" },
                new Branch {NameShort = "abc", Name = "Accounting, Banking and Compensation" }
            };

            branches.ForEach(b => context.Branches.Add(b));
            context.SaveChanges();
        }
    }
}