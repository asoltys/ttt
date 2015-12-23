namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<TransformationTimelineTool.DAL.TimelineContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TransformationTimelineTool.DAL.TimelineContext context)
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string name = "Admin";
            string role2 = "OPI";
            string password = "123456";

            if (!RoleManager.RoleExists(name))
            {
                var roleResult = RoleManager.Create(new IdentityRole(name));
            }

            if (!RoleManager.RoleExists(role2))
            {
                var roleResult = RoleManager.Create(new IdentityRole(role2));
            }

            var user = new User();
            user.UserName = name;

            var adminResult = UserManager.Create(user, password);

            if (adminResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }

            user = new User();

            user.UserName = "wongrm";

            var myResult = UserManager.Create(user, password);

            if (myResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }

            user = new User();
            user.UserName = "ttt_opi";
            myResult = UserManager.Create(user, password);

            if (myResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, role2);
            }
            user = new User();
            user.UserName = "ttt_editor";
            myResult = UserManager.Create(user, password);

            if (myResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, role2);
            }

            user = new User();
            user.UserName = "matty";
            myResult = UserManager.Create(user, password);

            base.Seed(context);
            
            /*
            var regions = new List<Region>
            {
                new Region {NameShort = "pac", NameE = "Pacific" },
                new Region {NameShort = "wst", NameE = "Western" },
                new Region {NameShort = "que", NameE = "Quebec" }
            };

            regions.ForEach(r => context.Regions.Add(r));
            context.SaveChanges();

            var branches = new List<Branch>
            {
                new Branch {NameShort = "rp", NameE = "Real Property" },
                new Branch {NameShort = "ciob", NameE = "Chief Information Officer Branch" },
                new Branch {NameShort = "abc", NameE = "Accounting, Banking and Compensation" }
            };

            branches.ForEach(b => context.Branches.Add(b));
            context.SaveChanges();

            var initiatives = new List<Initiative>
            {
                new Initiative {NameE = "Defense Procurement Strategy", DescriptionE = "Descp 1", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") },
                new Initiative {NameE = "Case and Client Relationship Management", DescriptionE = "Descp 2", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") },

            };

            initiatives.ForEach(i => context.Initiatives.Add(i));
            context.SaveChanges();

            var events = new List<Event>
            {
               new Event {InitiativeID = initiatives.Single(i => i.DescriptionE == "Descp 1").ID,
                   Date = DateTime.Parse("2015-07-31"),
                   BranchID = branches.Single(b => b.NameShort == "rp").ID,
                   RegionID = regions.Single(r => r.NameShort == "pac").ID,
                   TextE = "Launch of SMART Procurement",
                   HoverE = "Launch of SMART Procurement" },
               new Event {InitiativeID = initiatives.Single(i => i.DescriptionE == "Descp 1").ID,
                   Date = DateTime.Parse("2015-06-30"),
                   BranchID = branches.Single(b => b.NameShort == "ciob").ID,
                   RegionID = regions.Single(r => r.NameShort == "pac").ID,
                   TextE = "Launch of SMART Procurement Lean",
                   HoverE = "Launch of SMART Procurement Lean" },
               new Event {InitiativeID = initiatives.Single(i => i.DescriptionE == "Descp 1").ID,
                   Date = DateTime.Parse("2015-08-15"),
                   BranchID = branches.Single(b => b.NameShort == "rp").ID,
                   RegionID = regions.Single(r => r.NameShort == "wst").ID,
                   TextE = "Matty's 31st birthday",
                   HoverE = "Matty's 31st birthday" }
            };

            events.ForEach(e => context.Events.Add(e));
            context.SaveChanges();
            */
        }
    }
}
