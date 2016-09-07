namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRegion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegionID", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "RegionID");
            AddForeignKey("dbo.AspNetUsers", "RegionID", "dbo.Regions", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "RegionID", "dbo.Regions");
            DropIndex("dbo.AspNetUsers", new[] { "RegionID" });
            DropColumn("dbo.AspNetUsers", "RegionID");
        }
    }
}
