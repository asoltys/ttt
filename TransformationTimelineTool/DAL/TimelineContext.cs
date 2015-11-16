using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.Models;
using System.Data.Entity;

namespace TransformationTimelineTool.DAL
{
    public class TimelineContext : DbContext
    {
        public TimelineContext() : base("TimelineContext")
        {
        }
        public DbSet<Initiative> Initiatives { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Impact> Impacts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Event> Events { get; set; }
    }

}