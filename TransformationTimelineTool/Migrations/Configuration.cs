namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<TransformationTimelineTool.DAL.TimelineContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TransformationTimelineTool.DAL.TimelineContext context)
        {
            var regions = new List<Region>
            {
                new Region {NameShort = "pac", Name = "Pacific" },
                new Region {NameShort = "wst", Name = "Western" },
                new Region {NameShort = "wst", Name = "Western" },
                new Region {NameShort = "que", Name = "Quebec" }
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

            var initiatives = new List<Initiative>
            {
                new Initiative {NameE = "Defense Procurement Strategy", Description = "Descp 1", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") },
                new Initiative {NameE = "Case and Client Relationship Management", Description = "Descp 2", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") },

            };

            initiatives.ForEach(i => context.Initiatives.Add(i));
            context.SaveChanges();

            var events = new List<Event>
            {
               new Event {InitiativeID = 1, Date = DateTime.Parse("2015-07-31"), Text = "Launch of SMART Procurement", Hover = "Launch of SMART Procurement" },
               new Event {InitiativeID = 1, Date = DateTime.Parse("2015-06-28"), Text = "Launch of SMART Procurement LEAN Pilot", Hover = "Launch of SMART Procurement LEAN Pilot" },
               new Event {InitiativeID = 2, Date = DateTime.Parse("2015-07-31"), Text = "Online E-Learning courses offered", Hover = "Online E-Learning courses offered" },
               new Event {InitiativeID = 1, Date = DateTime.Parse("2015-08-15"), Text = "Birthday", Hover = "Birthday!" }
            };

            events.ForEach(e => context.Events.Add(e));
            context.SaveChanges();
        }
    }
}
