using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TransformationTimelineTool.DAL
{
    public class TimelineContext : IdentityDbContext<User>
    {
        public TimelineContext() : base("TimelineContext")
        {
        }
        public DbSet<Initiative> Initiatives { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Impact> Impacts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Edit> Edits { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }

        public System.Data.Entity.DbSet<TransformationTimelineTool.Models.Comment> Comments { get; set; }
    }

}