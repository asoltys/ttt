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

            string admin = "Admin";
            string opi = "OPI";
            string editor = "Editor";

            AddRole(RoleManager, admin);
            AddRole(RoleManager, opi);
            AddRole(RoleManager, editor);

            AddUser(UserManager, admin, admin, "PWGSC.PacificWebServices-ReseaudesServicesduPacifique.TPSGC@pwgsc-tpsgc.gc.ca");
            AddUser(UserManager, "wongrm", admin);
            AddUser(UserManager, "ttt_opi", opi);
            AddUser(UserManager, "ttt_editor", editor);
            AddUser(UserManager, "matty", admin);
            
            var regions = new List<Region>
            {
                new Region {NameShort = "all", NameE = "All", NameF = "Tout" },
                new Region {NameShort = "pac", NameE = "Pacific", NameF = "Pacifique" },
                new Region {NameShort = "wst", NameE = "Western", NameF = "Ouest" },
                new Region {NameShort = "ont", NameE = "Ontario", NameF = "Ontario" },
                new Region {NameShort = "nca", NameE = "NCA", NameF = "SCN" },
                new Region {NameShort = "atl", NameE = "Atlantic", NameF = "Atlantique" },
                new Region {NameShort = "que", NameE = "Quebec", NameF = "Québec" }
            };

            regions.ForEach(r => context.Regions.AddOrUpdate(p => p.NameShort, r));
            context.SaveChanges();

            var branches = new List<Branch>
            {
                new Branch {NameShort = "all", NameE = "All", NameF = "Tout" },
                new Branch {NameShort = "rp", NameE = "Real Property", NameF = "Biens immobiliers" },
                new Branch {NameShort = "abc", NameE = "Accounting, Banking and Compensation", NameF = "Comptabilité, gestion bancaire et rémunération " },
                new Branch {NameShort = "acqcomp", NameE = "Acquisitions", NameF = "Approvisionnements" },
                new Branch {NameShort = "ciob", NameE = "Chief Information Officer Branch", NameF = "Direction générale du dirigeant principal de l’information" },
                new Branch {NameShort = "do", NameE = "Departmental Oversight", NameF = "Surveillance" },
                new Branch {NameShort = "fab", NameE = "Finance and Administration Branch", NameF = "Finances et administration" },
                new Branch {NameShort = "hr", NameE = "Human Resources", NameF = "Ressources humaines" },
                new Branch {NameShort = "is", NameE = "Integrated Services", NameF = "Services intégrés" },
                new Branch {NameShort = "osme", NameE = "Office of Small and Medium Enterprises", NameF = "Bureau des petites et moyennes entreprises" },
                new Branch {NameShort = "pp", NameE = "Parliamentary Precinct", NameF = "Cité parlementaire" },
                new Branch {NameShort = "ppp", NameE = "Policy, Planning and Communications", NameF = "Politiques, planification et communications" },
                new Branch {NameShort = "tb", NameE = "Translation Bureau", NameF = "Bureau de la traduction" },
                new Branch {NameShort = "cssmc", NameE = "Corporate Services, Strategic Management and Communications", NameF = "Services ministériels, gestion stratégique et communications" },
                new Branch {NameShort = "rdgo", NameE = "Regional Director General’s Office", NameF = "Bureau du directeur général régional" }
            };

            branches.ForEach(b => context.Branches.AddOrUpdate(p => p.NameShort, b));
            context.SaveChanges();

            base.Seed(context);

           
 /*
            var initiatives = new List<Initiative>
            {
                new Initiative {NameE = "Defense Procurement Strategy", DescriptionE = "Descp 1", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") },
                new Initiative {NameE = "Case and Client Relationship Management", DescriptionE = "Descp 2", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") },
                new Initiative {NameE = "Holidays", DescriptionE = "Canadian Holidays", StartDate = DateTime.Parse("2013-12-1"), EndDate = DateTime.Parse("2020-3-31") }

            };

            initiatives.ForEach(i => context.Initiatives.AddOrUpdate(p => p.NameE, i));
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

        void AddUser(UserManager<User> userManager, string name, string role, string email = "mathieu.wong-rose@pwgsc.gc.ca")
        {

            var user = new User();
            user.UserName = name;
            user.Email = email;

            var addResult = userManager.Create(user, "123456");

            if (addResult.Succeeded)
            {
                var result = userManager.AddToRole(user.Id, role);
            }

        }

        void AddRole(RoleManager<IdentityRole> roleManager, string name)
        {
            if (!roleManager.RoleExists(name))
            {
                var roleResult = roleManager.Create(new IdentityRole(name));
            }
        }
    }
}
